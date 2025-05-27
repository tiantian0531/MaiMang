using System.Reflection;

namespace MaiMangKit.Proxy;

public class ServiceProxy : DispatchProxy
{
    public object? RealService { get; set; }

    public IInterceptor[]? Interceptors { get; set; }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) throw new ArgumentNullException(nameof(targetMethod));
        var context = new InvocationContext
        {
            TargetMethod = targetMethod,
            Arguments = args,
            RealService = RealService,
            Proceed = () => targetMethod.Invoke(RealService, args)
        };
        return InterceptorPipeline.Execute(context, Interceptors);
    }
}