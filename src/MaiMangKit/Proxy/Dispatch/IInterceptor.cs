using System.Reflection;

namespace MaiMangKit.Proxy;


// 调用上下文
public class InvocationContext
{
    public MethodInfo TargetMethod { get; set; } = null!;
    public object?[]? Arguments { get; set; }
    public object? RealService { get; set; }
    public Func<object?> Proceed { get; set; } = null!;
}

public interface IInterceptor
{
    object? Intercept(InvocationContext context, Func<object?> next);
}