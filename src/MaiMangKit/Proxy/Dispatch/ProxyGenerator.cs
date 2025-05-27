using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MaiMangKit.Proxy;

public static class ProxyGenerator
{
    public static TInterface Create<TInterface>(params IInterceptor[] interceptors)
        where TInterface : class
    {
        return Create<TInterface>(null, interceptors);
    }

    public static TInterface Create<TInterface>(TInterface? realService, params IInterceptor[] interceptors)
        where TInterface : class
    {
        var proxy = DispatchProxy.Create<TInterface, ServiceProxy>();
        var proxyInstance = (proxy as ServiceProxy)!;
        proxyInstance.RealService = realService;
        proxyInstance.Interceptors = interceptors;
        return proxy;
    }
}