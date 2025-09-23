namespace WebGESCOMPAH.Extensions
{
    public static class CorsService
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var fromConfig = (configuration.GetValue<string>("OrigenesPermitidos") ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin =>
                        {
                            if (string.Equals(origin, "capacitor://localhost", StringComparison.OrdinalIgnoreCase))
                                return true;

                            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                            {
                                if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                                    return true;

                                if (uri.Host.EndsWith(".ngrok-free.app", StringComparison.OrdinalIgnoreCase) ||
                                    uri.Host.EndsWith(".ngrok.app", StringComparison.OrdinalIgnoreCase))
                                    return true;
                            }

                            return fromConfig.Contains(origin, StringComparer.OrdinalIgnoreCase);
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }

}