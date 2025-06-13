namespace MaiMangCore.ElasticSearch;

public class ElasticsearchClientOptions
{
    /// <summary>
    /// 是否显示日志
    /// </summary>
    public bool ShowLogInfo { get; set; } = false;

    /// <summary>
    /// 服务节点
    /// </summary>
    public List<string> NodeUrls { get; set; }

    /// <summary>
    /// 索引前缀
    /// </summary>
    public string IndexPrefix { get; set; }

    /// <summary>
    /// 登录账号
    /// </summary>
    public string LoginName { get; set; }

    /// <summary>
    /// 登录密码（明文）
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 登录密码（加密）
    /// </summary>
    public string Base64PassPassword { get; set; }
}