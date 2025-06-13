using Microsoft.Extensions.DependencyInjection;

namespace MaiMangCore.ElasticSearch;

public  static class ElasticSearchServiceExtensions
{
    public static IServiceCollection AddElasticSearch(this IServiceCollection services, Action<ElasticsearchClientOptions> action)
    {
        services.Configure(action);
        services.AddSingleton<ConnectionFactory>();
        return services;
    }
}