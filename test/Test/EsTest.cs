using MaiMangCore.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Test.RequestLoggerRepository;

namespace Test;

public class ElasticSearchBasicRepository<T> : ElasticSearchContextAbstract<T> where T : IdBaseIndex, new()
{
    public ElasticSearchBasicRepository(ConnectionFactory connection) : base(connection)
    {
    }
}

public class RequestLoggerRepository : ElasticSearchBasicRepository<AddLoggerIndex>
{
    public RequestLoggerRepository(ConnectionFactory connection) : base(connection)
    {
    }

    public async Task<List<AddLoggerIndex>> GetLogsAsync(LoggerSearch input)
    {
        var retryCount = 0;
        do
        {
            var data = await Context.SearchAsync<AddLoggerIndex>(m => m
               .Query(q =>
                   (string.IsNullOrEmpty(input.BusinessTag1) ? null : q.Term(f => f.BusinessTag1.Suffix("keyword"), input.BusinessTag1)) &&
                   (string.IsNullOrEmpty(input.BusinessTag2) ? null : q.Term(f => f.BusinessTag2.Suffix("keyword"), input.BusinessTag2)) &&
                   (string.IsNullOrEmpty(input.BusinessTag3) ? null : q.Term(f => f.BusinessTag3.Suffix("keyword"), input.BusinessTag3)) &&
                   (string.IsNullOrEmpty(input.KeyWord) ? null : q.MatchPhrase(m => m.Field("content").Query(input.KeyWord))) &&
                   q.Term(p => p.Source, input.Source)
               )
               .Sort(s => s.Field(f => f.RequestTime, SortOrder.Descending))
               .From((input.PageIndex - 1) * input.PageSize)
               .Size(input.PageSize)
            );

            if (data.Documents.Count > 0)
            {
                var res = data.Documents.ToList();
                return res;
            }
            else
            {
                retryCount++;
                await Task.Delay(1000);
            }
        } while (retryCount < 2);

        return new List<AddLoggerIndex>();
    }
        
}


//[ElasticsearchType(RelationName = "indexName")]
public class AddLoggerIndex : IdBaseIndex
{
    /// <summary>
    /// 日志来源名称
    /// </summary>
    public string SourceName { get; set; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; set; }

    public string BusinessTag1 { get; set; }

    /// <summary>
    /// 业务标识2
    /// </summary>
    public string BusinessTag2 { get; set; }

    /// <summary>
    /// 业务标识3
    /// </summary>
    public string BusinessTag3 { get; set; }

    /// <summary>
    /// 日志来源
    /// </summary>
    public int Source { get; set; }

}


public class LoggerSearch
{
    /// <summary>
    /// 日志来源
    /// </summary>
    public int Source { get; set; }

    /// <summary>
    /// 业务标识1
    /// </summary>
    public string BusinessTag1 { get; set; }

    /// <summary>
    /// 业务标识2
    /// </summary>
    public string BusinessTag2 { get; set; }

    /// <summary>
    /// 业务标识3
    /// </summary>
    public string BusinessTag3 { get; set; }

    /// <summary>
    /// 关键字搜索，BusinessTag1、BusinessTag2、BusinessTag3
    /// </summary>
    public string KeyWord { get; set; }

    /// <summary>
    /// 当前页码，下标从1开始
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }
}

