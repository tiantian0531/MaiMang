namespace MaiMangKit.Extensions;

/// <summary>
/// 集合扩展
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 如果集合不为空并且有元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IfNotNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source != null && source.Any();
    }

    /// <summary>
    /// 如果集合为空并且没有元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IfNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// false -> todo
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="action"></param>
    public static void Then(this bool condition, Action action)
    {
        if (condition && action != null)
        {
            action();
        }
    }
}