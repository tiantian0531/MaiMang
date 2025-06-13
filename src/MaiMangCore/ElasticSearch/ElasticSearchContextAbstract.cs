using Nest;

namespace MaiMangCore.ElasticSearch;

public abstract class ElasticSearchContextAbstract<T> : IElasticsearchClientService<T> where T : IdBaseIndex, new()
{
    private readonly IElasticClient _client;
    private readonly string _indexName;

    protected ElasticSearchContextAbstract(ConnectionFactory connectionFactory)
    {
        _client = connectionFactory.GetClient(typeof(T));
        _indexName = _client.ConnectionSettings.DefaultIndex;
    }

    public IElasticClient Context => _client;

    public virtual async Task<string> IndexDocumentAsync(T data)
    {
        var response = await ExecuteWithExceptionHandling(() =>
            Context.IndexAsync(data, x => x.Index(_indexName)));

        return response.IsValid ? response.Id : throw new Exception($"数据添加失败：{response.DebugInformation}");
    }


    public virtual async Task<BulkResponse> IndexManyDocumentAsync(List<T> list, bool refresh = false)
    {
        var response = await ExecuteWithExceptionHandling(() =>
            Context.IndexManyAsync(list, _indexName));

        if (refresh && response.IsValid)
        {
            await Context.Indices.RefreshAsync(_indexName);
        }

        return response;
    }

    public virtual async Task DeleteByIdAsync(string id)
    {
        var response = await ExecuteWithExceptionHandling(() =>
            Context.DeleteAsync<T>(id, x => x.Index(_indexName)));

        if (!response.IsValid)
        {
            throw new Exception($"数据删除失败，原因：{response.DebugInformation}");
        }
    }


    public virtual async Task UpdateAsync(string id, T data)
    {
        var response = await ExecuteWithExceptionHandling(() =>
            Context.UpdateAsync<T>(id, u => u.Doc(data).Index(_indexName)));

        if (!response.IsValid)
        {
            throw new Exception($"数据修改失败，原因：{response.DebugInformation}");
        }
    }


    public virtual async Task<T> GetByIdAsync(string id)
    {
        var response = await ExecuteWithExceptionHandling(() =>
            Context.GetAsync<T>(id, x => x.Index(_indexName)));

        if (response.Source != null)
        {
            response.Source.Id = response.Id;
        }

        return response.Source ?? throw new Exception($"数据查询失败，原因：未找到ID为 {id} 的文档");
    }

    private async Task<TResponse> ExecuteWithExceptionHandling<TResponse>(Func<Task<TResponse>> operation)
    {
        var response = await operation();
        // 检查是否实现了IResponse接口，这个接口所有响应都实现
        if (response is IResponse resp && !resp.IsValid)
        {
            throw new Exception($"Elasticsearch 操作失败: {resp.DebugInformation}");
        }

        return response;
    }
}