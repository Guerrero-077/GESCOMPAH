using Microsoft.Extensions.DependencyInjection;
using WebGESCOMPAH.Middleware;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class ExceptionHandlersModuleExtensions
    {
        public static IServiceCollection AddExceptionHandlersModule(this IServiceCollection services)
        {
            var webAsm = typeof(ExceptionMiddleware).Assembly;

            services.Scan(scan => scan
                .FromAssemblies(webAsm)
                .AddClasses(c => c.AssignableTo<IExceptionHandler>())
                    .As<IExceptionHandler>()
                    .WithSingletonLifetime()
            );

            return services;
        }
    }
}

