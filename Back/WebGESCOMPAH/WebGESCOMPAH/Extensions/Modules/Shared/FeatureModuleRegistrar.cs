using System.Reflection;

namespace WebGESCOMPAH.Extensions.Modules.Shared
{
    /// <summary>
    /// Registro DI genérico para features del dominio usando Scrutor.
    /// </summary>
    /// <remarks>
    /// Qué hace:
    /// - Escanea ensamblados de <c>Business</c> y opcionalmente <c>Data</c> para registrar
    ///   automáticamente clases del namespace <c>Services.&lt;Feature&gt;</c> como servicios
    ///   (excluye tipos que terminan en <c>Repository</c>), y registra repositorios que
    ///   coincidan por convención.
    /// 
    /// Por qué:
    /// - Evita duplicar lógica de <c>services.Scan(...)</c> en cada módulo de la WebApp.
    /// - Garantiza reglas consistentes (scope, convenciones de nombres/namespace).
    /// 
    /// Para qué:
    /// - Ser llamada desde los distintos <c>*ModuleExtensions</c> (Business, Persons, etc.) y
    ///   mantener el arranque limpio y DRY.
    /// </remarks>
    public static class FeatureModuleRegistrar
    {
        /// <summary>
        /// Registra servicios (no repositorios) y repositorios para un módulo/feature dado.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="businessAssembly">Assembly ancla donde están los servicios de negocio</param>
        /// <param name="dataAssembly">Assembly ancla donde están los repositorios (puede ser null)</param>
        /// <param name="servicesNamespaceSuffix">Sufijo de namespace bajo <c>Services.*</c> (p.ej. <c>"Business"</c>, <c>"Location"</c>, <c>"Persons"</c>).</param>
        /// <param name="extraRepositoryNamespaces">Namespaces adicionales que también contienen repositorios que deben registrarse.</param>
        public static IServiceCollection AddFeatureModule(
            this IServiceCollection services,
            Assembly businessAssembly,
            Assembly? dataAssembly,
            string servicesNamespaceSuffix,
            params string[] extraRepositoryNamespaces)
        {
            var assemblies = dataAssembly is null
                ? new[] { businessAssembly }
                : new[] { businessAssembly, dataAssembly };

            services.Scan(scan =>
                scan.FromAssemblies(assemblies)
                    // Servicios de negocio del feature (excluye repositorios)
                    .AddClasses(c => c.Where(t => t.Namespace != null
                                                  && t.Namespace.Contains($"Services.{servicesNamespaceSuffix}")
                                                  && !t.Name.EndsWith("Repository")))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
                    // Repositorios del feature (en Services.<feature> y opcionalmente en otros namespaces)
                    .AddClasses(c => c.Where(t =>
                    {
                        if (t.Namespace is null) return false;
                        if (!t.Name.EndsWith("Repository")) return false;

                        var ns = t.Namespace;
                        if (ns.Contains($"Services.{servicesNamespaceSuffix}"))
                            return true;

                        foreach (var extra in extraRepositoryNamespaces)
                        {
                            if (!string.IsNullOrWhiteSpace(extra) && ns.Contains(extra))
                                return true;
                        }
                        return false;
                    }))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
            );

            return services;
        }
    }
}

