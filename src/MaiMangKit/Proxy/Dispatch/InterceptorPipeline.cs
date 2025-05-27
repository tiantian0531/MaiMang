using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiMangKit.Proxy;

public static class InterceptorPipeline
{
    public static object? Execute(InvocationContext context, IInterceptor[]? interceptors, int index = 0)
    {
        if (interceptors == null || index >= interceptors.Length)
        {
            return context.Proceed();
        }

        var interceptor = interceptors[index];
        return interceptor.Intercept(context, () => Execute(context, interceptors, index + 1));
    }
}