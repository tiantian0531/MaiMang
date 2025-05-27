using MaiMangKit.Proxy;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Test
{

    // 定义业务接口
    public interface IUserService
    {
        string GetUserName(int userId);
    }

    // 日志拦截器
    public class LogInterceptor : IInterceptor
    {
        public object? Intercept(InvocationContext context, Func<object?> next)
        {
            Console.WriteLine($"调用日志方法: {context.TargetMethod.Name}, 参数: {string.Join(", ", context.Arguments ?? Array.Empty<object>())}");
            var result = next();
            Console.WriteLine($"返回结果: {result}");
            return result;
        }
    }
    public class CachingInterceptor : IInterceptor
    {
        public object? Intercept(InvocationContext context, Func<object?> next)
        {
            Console.WriteLine("缓存拦截器开始");
            object result = next();
            Console.WriteLine("缓存拦截器结束");
            return result;
        }
    }
    // 真实服务实现
    public class UserService : IUserService
    {
        public string GetUserName(int userId)
        {
            Console.WriteLine("执行业务方法");
            return  $"User_{userId}";
        }

     
       
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var realService = new UserService();
            var proxy = ProxyGenerator.Create<IUserService>(
                realService,
                new LogInterceptor(),
                new CachingInterceptor()
            );

            var userName = proxy.GetUserName(1); // 
        }
        //public async Task TestAnalyzeMethodNotIncludeMethod()
        //{
        //    await Task.Delay(1000);

        //    Task M1() { return Task.CompletedTask; }
        //    Task M2() { return Task.CompletedTask; }
        //    Task M3() { return Task.CompletedTask; }
        //}

        //public static async void TestAnalyzeAsyncMethodNotReturnVoid()
        //{
        //    await Task.Delay(1000);
        //}

        [SuppressMessage("Naming", "MMSTYLE001")]
        public void a() { }
        //public void a1() { }


    }


}
