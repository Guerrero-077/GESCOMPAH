using Business.CQRS.Auth.Commands.Login;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.AdministrationSystem;
using Entity.DTOs.Implements.Persons.Peron;
using Entity.DTOs.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Mapster;

namespace Business.Mapping
{
    public static class MapsterConfig
    {
        public static TypeAdapterConfig Register()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // Map RegisterDto → User
            config.NewConfig<RegisterDto, User>()
                .Ignore(dest => dest.Id);

            // Map RegisterDto → Person
            config.NewConfig<RegisterDto, Person>()
                .Ignore(dest => dest.Id);

            // Map User → UserDto
            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Person, src => src.Person)
                .Map(dest => dest.Roles, src => src.RolUsers.Select(r => r.Rol.Name).ToList());

            // Map Person → PersonDto
            config.NewConfig<Person, PersonDto>();

            // ---------------------------
            // Map LoginCommand → LoginDto
            // (útil para CQRS, aunque es herencia, queda explícito)
            config.NewConfig<LoginCommand, LoginDto>();

            // NUEVO: Mapear Module → MenuModuleDto
            config.NewConfig<Module, MenuModuleDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Forms, src => src.FormModules.Select(fm => fm.Form).Adapt<List<FormDto>>());

            // Mapear Form → FormDto (incluye permisos manualmente en handler, pero puede incluirse)
            config.NewConfig<Form, FormDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                // .Permissions será asignado en el handler manualmente porque es dinámica

                ;

            // Opcional: Mapear User → UserMeDto si quieres automatizar parcialmente
            config.NewConfig<User, UserMeDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FullName, src => $"{src.Person.FirstName} {src.Person.LastName}")
                .Map(dest => dest.Email, src => src.Email)
                // Roles y Permissions asignados en handler manualmente
                ;

            return config;
        }
    }
}
