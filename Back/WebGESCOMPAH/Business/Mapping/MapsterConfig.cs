using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.AdministrationSystem;
using Entity.DTOs.Implements.Business.Establishment;
using Entity.DTOs.Implements.Business.Plaza;
using Entity.DTOs.Implements.Persons.Peron;
using Entity.DTOs.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Entity.DTOs.Implements.Utilities.Images;

using Mapster;

namespace Business.Mapping
{
    public static class MapsterConfig
    {
        public static TypeAdapterConfig Register()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // 🌐 Usuario / Autenticación
            config.NewConfig<RegisterDto, User>()
                .Ignore(dest => dest.Id);

            config.NewConfig<RegisterDto, Person>()
                .Ignore(dest => dest.Id);

            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Person, src => src.Person)
                .Map(dest => dest.Roles, src => src.RolUsers.Select(r => r.Rol.Name).ToList());

            config.NewConfig<User, UserMeDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FullName, src => $"{src.Person.FirstName} {src.Person.LastName}")
                .Map(dest => dest.Email, src => src.Email);

            config.NewConfig<Person, PersonDto>();

            // 📦 Plazas
            config.NewConfig<Plaza, PlazaSelectDto>();

            // 🧩 Módulos / Formularios
            config.NewConfig<Module, MenuModuleDto>()
                .Map(dest => dest.Forms, src => src.FormModules.Select(fm => fm.Form).Adapt<List<FormDto>>());

            config.NewConfig<Form, FormDto>();

            // 🏢 Establecimientos
            config.NewConfig<Establishment, EstablishmentSelectDto>()
                .Map(dest => dest.Images, src => src.Images.Adapt<List<ImageSelectDto>>());

            config.NewConfig<EstablishmentCreateDto, Establishment>()
                .Ignore(dest => dest.Images); // Se cargan después

            config.NewConfig<EstablishmentUpdateDto, Establishment>()
                .Ignore(dest => dest.Images); // Se maneja manualmente

            // 🖼️ Imágenes
            config.NewConfig<Images, ImageCreateDto>(); // Inverso no lo usás, pero lo dejas si alguna vez lo necesitás

            config.NewConfig<Images, ImageSelectDto>();

            config.NewConfig<ImageUpdateDto, Images>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.FilePath)     // solo se modifica desde Cloudinary
                .Ignore(dest => dest.FileName)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.IsDeleted)    // controlado por lógica

                // Esta es la clave para evitar sobrescritura con null
                .IgnoreNullValues(true);

            return config;
        }
    }
}
