using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Location;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using Entity.DTOs.Implements.Business.Appointment;
using Entity.DTOs.Implements.Business.Contract;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.DTOs.Implements.Business.Plaza;
using Entity.DTOs.Implements.Business.PremisesLeased;
using Entity.DTOs.Implements.Location.City;
using Entity.DTOs.Implements.Location.Department;
using Entity.DTOs.Implements.Persons.Person;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
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
            // AdministrationSystem
            // ============================================

            config.NewConfig<Form, FormSelectDto>();
            config.NewConfig<FormModule, FormModuleSelectDto>();
            config.NewConfig<Module, ModuleSelectDto>();
            config.NewConfig<SystemParameter, SystemParameterDto>();
            config.NewConfig<SystemParameter, SystemParameterUpdateDto>();

            // ============================================
            // Business
            // ============================================

            config.NewConfig<Appointment, AppointmentSelectDto>()
                    .Map(dest => dest.EstablishmentName, src => src.Establishment.Name);


            // Entidad → DTO de selección
            config.NewConfig<Establishment, EstablishmentSelectDto>()
                .Map(dest => dest.Images, src => src.Images.Adapt<List<ImageSelectDto>>());

            // DTO de creación → Entidad
            config.NewConfig<EstablishmentCreateDto, Establishment>()
                .Ignore(dest => dest.Id)               // Se genera en DB
                .Ignore(dest => dest.Images)           // Se manejan aparte
                .Ignore(dest => dest.Active)           // Control de negocio
                .IgnoreNullValues(true);

            // DTO de actualización → Entidad (ignorar nulos y valores por defecto)
            config.NewConfig<EstablishmentUpdateDto, Establishment>()
                .Ignore(dest => dest.Images)   // Se manejan aparte
                .Ignore(dest => dest.Active)   // No se actualiza desde DTO
                .IgnoreNullValues(true);



            config.NewConfig<Plaza, PlazaSelectDto>();






            TypeAdapterConfig<ContractCreateDto, Person>.NewConfig()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Document, src => src.Document)
                .Map(dest => dest.Phone, src => src.Phone)
                .Map(dest => dest.Address, src => src.Address)
                .Map(dest => dest.CityId, src => src.CityId);

            TypeAdapterConfig<ContractCreateDto, Contract>.NewConfig()
                .Map(dest => dest.StartDate, src => src.StartDate)
                .Map(dest => dest.EndDate, src => src.EndDate);

            //TypeAdapterConfig<ContractCreateDto, ContractTerms>.NewConfig()
            //    .Map(dest => dest.UvtQty, src => src.UvtQty)
            //    .Map(dest => dest.UseSystemParameters, src => src.UseSystemParameters);


            TypeAdapterConfig<Contract, ContractSelectDto>.NewConfig()
                .Map(dest => dest.FullName, src => src.Person.FirstName + " " + src.Person.LastName)
                .Map(dest => dest.Document, src => src.Person.Document)
                .Map(dest => dest.Phone, src => src.Person.Phone)
                .Map(dest => dest.PremisesLeased, src => src.PremisesLeased)
                .Map(dest => dest.Email, src => src.Person.User != null ? src.Person.User.Email : string.Empty);

                //.Map(dest => dest.Terms, src => src.ContractTerms.FirstOrDefault());




            config.NewConfig<PremisesLeased, PremisesLeasedSelectDto>()
                .Map(dest => dest.EstablishmentId, src => src.Establishment.Id)
                .Map(dest => dest.EstablishmentName, src => src.Establishment.Name)
                .Map(dest => dest.Description, src => src.Establishment.Description)
                .Map(dest => dest.AreaM2, src => src.Establishment.AreaM2)
                .Map(dest => dest.RentValueBase, src => src.Establishment.RentValueBase)
                .Map(dest => dest.Address, src => src.Establishment.Address)
                .Map(dest => dest.PlazaName, src => src.Establishment.Plaza.Name)
                .Map(dest => dest.Images, src => src.Establishment.Images);








            // ============================================
            // Location
            // ============================================

            config.NewConfig<City, CitySelectDto>();

            config.NewConfig<Department, DepartmentSelectDto>();


            // ============================================
            // Persons
            // ============================================
            // Entidad -> DTO de selección (maneja City nullable)

            //config.NewConfig<Person, PersonSelectDto>()
            //    .Map(dest => dest.CityName, src => src.City != null ? src.City.Name : string.Empty);

            config.NewConfig<Person, PersonSelectDto>()
                .Map(dest => dest.CityName, src => src.City != null ? src.City.Name : string.Empty)
                .Map(dest => dest.Email, src => src.User != null ? src.User.Email : null);


            // DTO de creación -> Entidad  (NO toca navegaciones)
            config.NewConfig<PersonDto, Person>()
                .Ignore(dest => dest.Id)    // lo genera DB
                .Ignore(dest => dest.City)  // navegación
                .IgnoreNullValues(false);   // para creación, aplica TODOS los campos

            // DTO de actualización -> Entidad (patch-friendly)
            config.NewConfig<PersonUpdateDto, Person>()
                .Ignore(dest => dest.Id)  // Ignora el Id para evitar modificar la clave primaria
                .Ignore(dest => dest.City)  // navegación
                .IgnoreNullValues(true);    // no pises con nulls

            // ============================================
            // SecurityAuthentication
            // ============================================

            config.NewConfig<Permission, PermissionSelectDto>();

            config.NewConfig<Rol, RolSelectDto>();

            config.NewConfig<RolFormPermission, RolFormPermissionSelectDto>();

            config.NewConfig<RolUser, RolUserSelectDto>();

            config.NewConfig<User, UserSelectDto>()
               .Map(dest => dest.PersonName, src => $"{src.Person.FirstName} {src.Person.LastName}")
               .Map(dest => dest.PersonId, src => src.PersonId)
               .Map(dest => dest.PersonDocument, src => src.Person.Document)
               .Map(dest => dest.PersonAddress, src => src.Person.Address)
               .Map(dest => dest.PersonPhone, src => src.Person.Phone)
               .Map(dest => dest.CityId, src => src.Person.CityId)
               .Map(dest => dest.CityName, src => src.Person.City != null ? src.Person.City.Name : string.Empty)
               .Map(dest => dest.Email, src => src.Email)
               .Map(dest => dest.Active, src => !src.IsDeleted);

            config.NewConfig<UserCreateDto, User>()
                .Ignore(dest => dest.Id);


            config.NewConfig<UserCreateDto, Person>()
                .Ignore(dest => dest.Id);

            config.NewConfig<UserUpdateDto, User>();

            config.NewConfig<UserUpdateDto, Person>()
                .Ignore(dest => dest.Id) // No modificar la PK
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Phone, src => src.Phone)
                .Map(dest => dest.Address, src => src.Address)
                .Map(dest => dest.CityId, src => src.CityId);




            // ============================================
            // Utilities
            // ============================================

            config.NewConfig<Images, ImageSelectDto>();


            //? Revisar

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



            // ============================================
            // USUARIOS / AUTENTICACIÓN
            // ============================================

            config.NewConfig<RegisterDto, User>()
                .Ignore(dest => dest.Id); 

            
            config.NewConfig<RegisterDto, Person>()
                .Ignore(dest => dest.Id);



            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Person, src => src.Person)
                .Map(dest => dest.Roles, src => src.RolUsers.Select(r => r.Rol.Name).ToList());

            // User hacia DTO para "mi perfil"
            config.NewConfig<User, UserMeDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FullName, src => $"{src.Person.FirstName} {src.Person.LastName}")
                .Map(dest => dest.Email, src => src.Email);

         
            config.NewConfig<Module, MenuModuleDto>()
                .Map(dest => dest.Forms, src => src.FormModules.Select(fm => fm.Form).Adapt<List<FormDto>>());

            config.NewConfig<Form, FormDto>();


            return config;
        }
    }
}
