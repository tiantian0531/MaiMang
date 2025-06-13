using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System.Collections.Concurrent;
using System.Reflection;

namespace MaiMangCore.ElasticSearch;

public class ConnectionFactory
{
    private static readonly object LockObj = new();
    private static ElasticsearchClientOptions _options;
    private static ConnectionSettings _connectionSettings;
    private static readonly ConcurrentDictionary<Type, IElasticClient> ClientCache = new();

    public ConnectionFactory(IOptions<ElasticsearchClientOptions> options)
    {
        if (_options == null)
        {
            _options = options.Value;
        }

        InitializeConnectionSettings();
    }

    private void InitializeConnectionSettings()
    {
        if (_connectionSettings != null) return;

        lock (LockObj)
        {
            if (_connectionSettings != null) return;

            var uris = _options?.NodeUrls?
                .Select(url => new Uri(url))
                .ToList() ?? throw new InvalidOperationException("未配置Elasticsearch节点地址");

            //using var connectionPool = new SniffingConnectionPool(uris);//集群连接池，支持嗅探，ping

            // 使用单节点连接池
            var connectionPool = new SingleNodeConnectionPool(uris[0]);

            var settings = new ConnectionSettings(connectionPool);

            if (!string.IsNullOrEmpty(_options.LoginName) && !string.IsNullOrEmpty(_options.Password))
            {
                settings = settings.BasicAuthentication(_options.LoginName, _options.Password);
            }

            _connectionSettings = settings;
        }
    }

    public IElasticClient GetClient<T>() where T : class
    {
        return GetClient(typeof(T));
    }

    public IElasticClient GetClient(Type type)
    {
        if (ClientCache.TryGetValue(type, out var cachedClient))
        {
            return cachedClient;
        }

        var newIndexName = DetermineIndexName(type);
        var settings = _connectionSettings.DefaultIndex(newIndexName);

        IElasticClient client;

        if (_options.ShowLogInfo)
        {
            settings.EnableDebugMode();
            client = new ElasticClient(settings.DisableDirectStreaming()
                .OnRequestCompleted(details =>
                {
                    string log = FormatApiCallDetails(details);
                    Console.WriteLine(log);
                }));
        }
        else
        {
            client = new ElasticClient(settings);
        }

        ClientCache.TryAdd(type, client);
        return client;
    }

    private string DetermineIndexName(Type type)
    {
        var elasticTypeAttribute = type.GetCustomAttribute<ElasticsearchTypeAttribute>(false);
        if (elasticTypeAttribute == null)
        {
            throw new InvalidOperationException($"类型 {type.FullName} 缺少 [ElasticsearchType] 属性，请指定 RelationName");
        }

        var usePrefixAttribute = type.GetCustomAttributes(typeof(EnableIndexPrefix), false).Any();

        if (usePrefixAttribute && string.IsNullOrEmpty(_options.IndexPrefix))
        {
            throw new InvalidOperationException(
                $"类型 {type.FullName} 启用了 IndexPrefixAttribute，但未在配置中找到 IndexPrefix，请检查配置");
        }

        return usePrefixAttribute ? $"{_options.IndexPrefix}{elasticTypeAttribute.RelationName}" : elasticTypeAttribute.RelationName;
    }

    private string FormatApiCallDetails(IApiCallDetails details)
    {
        return $@"
[执行时间]：{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}
[请求响应]：{details.DebugInformation}";
    }
}