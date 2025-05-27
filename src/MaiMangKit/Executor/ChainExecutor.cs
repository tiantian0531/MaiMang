using System.Collections;

namespace MaiMangKit.Executor;

/// <summary>
/// ChainExecutor
/// </summary>
public static class ChainExecutor
{
    /// <summary>
    ///  构建一个简单的责任链执行器
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static InvokeHandler<TParam, TResult> For<TParam, TResult>()
    {
        return new InvokeHandler<TParam, TResult>();
    }
}

/// <summary>
/// FallbackExecutor
/// </summary>
/// <typeparam name="TParam"></typeparam>
/// <typeparam name="TResult"></typeparam>
public class InvokeHandler<TParam, TResult>
{
    private readonly List<Func<TParam, Task<TResult>>> _handlers = new();
    private Func<TResult, bool> _validation = DefaultValidation;

    /// <summary>
    /// 添加一个处理器
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public InvokeHandler<TParam, TResult> AddHandler(Func<TParam, Task<TResult>> handler)
    {
        _handlers.Add(handler);
        return this;
    }

    /// <summary>
    /// 用于设置自定义中断验证器
    /// </summary>
    /// <param name="validation"></param>
    /// <returns></returns>
    public InvokeHandler<TParam, TResult> SetValidation(Func<TResult, bool> validation)
    {
        _validation = validation ?? DefaultValidation;
        return this;
    }

    /// <summary>
    ///  easy call
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<TResult> ExecuteAsync(TParam param)
    {
        if (_handlers.Count == 0) throw new InvalidOperationException("未注册任何处理器");

        foreach (var handler in _handlers)
        {
            try
            {
                var result = await handler(param);
                if (_validation(result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                continue;
            }
        }
        return default;
    }

    private static bool DefaultValidation(TResult result)
    {
        if (result == null) return false;
        if (result is string str) return !string.IsNullOrEmpty(str);
        if (result is IEnumerable enumerable) return enumerable.Cast<object>().Any();
        return true;
    }
}