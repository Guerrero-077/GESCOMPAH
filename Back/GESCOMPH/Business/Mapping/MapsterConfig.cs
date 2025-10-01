using System.Reflection;

using Business.Mapping.Registers;
using Mapster;

namespace Business.Mapping
{
    public static class MapsterConfig
    {
        public static TypeAdapterConfig Register()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // Aggregate all IRegister mappings in this assembly
            config.Scan(typeof(AdministrationSystemMapping).GetTypeInfo().Assembly);

            return config;
        }
    }
}

