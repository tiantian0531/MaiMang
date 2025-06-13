namespace MaiMangCore.ElasticSearch;
#pragma warning disable CS8618

[AttributeUsage(AttributeTargets.Class)]
public class EnableIndexPrefix : Attribute
{
    public EnableIndexPrefix()
    {
    }

    public EnableIndexPrefix(string indexPrefix)
    {
        Prefix = indexPrefix;
    }

    public string Prefix { get; }
}