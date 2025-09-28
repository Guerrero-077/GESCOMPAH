using Business.Services.Persons;
using Data.Services.Persons;
using WebGESCOMPAH.Extensions.Modules.Shared;

namespace WebGESCOMPAH.Extensions.Modules.Persons
{
    /// <summary>
    /// Registro DI del módulo de Personas.
    /// </summary>
    /// <remarks>
    /// Registra servicios/repos bajo Services.Persons con AddFeatureModule.
    /// </remarks>
    public static class PersonsModuleExtensions
    {
        public static IServiceCollection AddPersonsModule(this IServiceCollection services)
        {
            var businessAsm = typeof(PersonService).Assembly; // Business.Services.Persons
            var dataAsm = typeof(PersonRepository).Assembly;   // Data.Services.Persons

            return services.AddFeatureModule(businessAsm, dataAsm, "Persons");
        }
    }
}
