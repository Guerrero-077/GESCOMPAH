using WebGESCOMPAH.Middleware;

namespace WebGESCOMPAH.Extensions.Modules.Exceptions
{
    /// <summary>
    /// Registro DI de handlers de excepciones personalizados del middleware.
    /// </summary>
    /// <remarks>
    /// Escanea el ensamblado Web y registra todas las implementaciones de IExceptionHandler como singleton.
    /// </remarks>
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
