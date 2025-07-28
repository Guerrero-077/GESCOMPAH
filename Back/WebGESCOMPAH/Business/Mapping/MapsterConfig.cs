using DocumentFormat.OpenXml.Wordprocessing;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.Implements.Utilities;

using Entity.DTOs.Implements.AdministrationSystem;
using Entity.DTOs.Implements.Business.Appointment;
using Entity.DTOs.Implements.Business.EstablishmentDto;
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

            // ============================================
            // 🌐 USUARIOS / AUTENTICACIÓN
            // ============================================

            // DTO de registro hacia entidad User
            config.NewConfig<RegisterDto, User>()
                .Ignore(dest => dest.Id); // ID generado por DB

            // DTO de registro hacia entidad Person
            config.NewConfig<RegisterDto, Person>()
                .Ignore(dest => dest.Id); // ID generado por DB

            // User hacia DTO de retorno
            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Person, src => src.Person)
                .Map(dest => dest.Roles, src => src.RolUsers.Select(r => r.Rol.Name).ToList());

            // User hacia DTO para "mi perfil"
            config.NewConfig<User, UserMeDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FullName, src => $"{src.Person.FirstName} {src.Person.LastName}")
                .Map(dest => dest.Email, src => src.Email);

            // Person hacia DTO
            config.NewConfig<Person, PersonDto>();

            // ============================================
            // 📦 PLAZAS
            // ============================================
            config.NewConfig<Plaza, PlazaSelectDto>();

            // ============================================
            // 🧩 MÓDULOS Y FORMULARIOS
            // ============================================
            config.NewConfig<Module, MenuModuleDto>()
                .Map(dest => dest.Forms, src => src.FormModules.Select(fm => fm.Form).Adapt<List<FormDto>>());

            config.NewConfig<Form, FormDto>();

            // ============================================
            // 🏢 ESTABLECIMIENTOS
            // ============================================

            // Entidad → DTO para retorno con imágenes
            config.NewConfig<Establishment, EstablishmentSelectDto>()
                .Map(dest => dest.Images, src => src.Images.Adapt<List<ImageSelectDto>>());

            // DTO de creación → Entidad (las imágenes se manejan aparte)
            config.NewConfig<EstablishmentCreateDto, Establishment>()
                .Ignore(dest => dest.Images);

            // DTO de actualización → Entidad
            config.NewConfig<EstablishmentUpdateDto, Establishment>()
                .Ignore(dest => dest.Images) // Se maneja a mano en el servicio
                .IgnoreNullValues(true);     // Para evitar que nulls sobrescriban valores existentes

            // ============================================
            // 🖼️ IMÁGENES
            // ============================================

            // Entidad → DTO de retorno
            config.NewConfig<Images, ImageSelectDto>();

            // DTO de creación → Entidad (nuevo mapeo agregado)
            config.NewConfig<ImageCreateDto, Images>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Establishment) // Relación se setea manualmente
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.IsDeleted);

            // Entidad → DTO de creación (por si lo necesitás)
            config.NewConfig<Images, ImageCreateDto>();

            // DTO de actualización → Entidad
            config.NewConfig<ImageUpdateDto, Images>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.FilePath)    // Solo cambia desde Cloudinary
                .Ignore(dest => dest.FileName)    // No se modifica directamente
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.IsDeleted)   // Controlado por lógica
                .IgnoreNullValues(true);          // Fundamental para evitar sobrescritura


            config.NewConfig<Appointment, AppointmentSelectDto>()
                .Map(dest => dest.EstablishmentName, src => src.Establishment.Name);



            return config;
        }
    }
}
