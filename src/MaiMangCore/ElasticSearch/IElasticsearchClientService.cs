using Nest;

namespace MaiMangCore.ElasticSearch;

/// <summary>
/// 代表一个通用的Elasticsearch客户端服务接口，用于对指定类型的文档执行CRUD操作。
/// </summary>
/// <typeparam name="T">文档的数据模型类型，必须是一个引用类型。</typeparam>
public interface IElasticsearchClientService<T> where T : class
{
    /// <summary>
    /// 异步索引（保存）一个新的文档到Elasticsearch中。
    /// </summary>
    /// <param name="data">要索引的文档数据。</param>
    /// <returns>返回包含操作结果或文档ID的字符串。</returns>
    Task<string> IndexDocumentAsync(T data);

    /// <summary>
    /// 根据文档ID异步获取文档。
    /// </summary>
    /// <param name="Id">文档的唯一标识符。</param>
    /// <returns>返回找到的文档实例，如果未找到则可能返回null。</returns>
    Task<T> GetByIdAsync(string Id);

    /// <summary>
    /// 根据文档ID异步更新文档。
    /// </summary>
    /// <param name="id">文档的唯一标识符。</param>
    /// <param name="input">新的文档数据。</param>
    /// <returns>返回表示操作完成的任务。</returns>
    Task UpdateAsync(string id, T input);

    /// <summary>
    /// 根据文档ID异步删除文档。
    /// </summary>
    /// <param name="id">文档的唯一标识符。</param>
    /// <returns>返回表示操作完成的任务。</returns>
    Task DeleteByIdAsync(string id);

    /// <summary>
    /// 异步批量索引（保存）多个文档到Elasticsearch中。
    /// </summary>
    /// <param name="list">要索引的文档列表。</param>
    /// <param name="refresh">是否在操作后立即刷新相关索引以使文档可搜索。</param>
    /// <returns>返回包含批量操作结果的BulkResponse对象。</returns>
    Task<BulkResponse> IndexManyDocumentAsync(List<T> list, bool refresh);
}