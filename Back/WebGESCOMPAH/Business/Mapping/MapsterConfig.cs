using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.Persons;
using Entity.DTOs.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Mapster;

namespace Business.Mapping
{
    public static class MapsterConfig
    {
        public static TypeAdapterConfig Register()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // Map RegisterUserDto → User
            config.NewConfig<RegisterDto, User>()
                .Ignore(dest => dest.Id);

            // Map RegisterUserDto → Person
            config.NewConfig<RegisterDto, Person>()
                .Ignore(dest => dest.Id);

            // Map User → UserDto
            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Person, src => src.Person)
                .Map(dest => dest.Roles, src => src.RolUsers.Select(r => r.Rol.Name).ToList());

            // Map Person → PersonDto
            config.NewConfig<Person, PersonDto>();

            return config;
        }
    }
}
