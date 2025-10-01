using System.Linq;

using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.Implements.Persons;

using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using Entity.DTOs.Implements.SecurityAuthentication.User;

using Mapster;

namespace Business.Mapping.Registers
{
    public class SecurityAuthenticationMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Permission, PermissionSelectDto>();
            config.NewConfig<Rol, RolSelectDto>();
            config.NewConfig<RolFormPermission, RolFormPermissionSelectDto>();
            config.NewConfig<RolUser, RolUserSelectDto>();

            config.NewConfig<User, UserSelectDto>()
               .Map(dest => dest.PersonName, src => src.Person != null ? $"{src.Person.FirstName} {src.Person.LastName}".Trim() : string.Empty)
               .Map(dest => dest.PersonId, src => src.PersonId)
               .Map(dest => dest.PersonDocument, src => src.Person != null ? src.Person.Document : string.Empty)
               .Map(dest => dest.PersonAddress, src => src.Person != null ? src.Person.Address : string.Empty)
               .Map(dest => dest.PersonPhone, src => src.Person != null ? src.Person.Phone : string.Empty)
               .Map(dest => dest.CityId, src => src.Person != null ? src.Person.CityId : 0)
               .Map(dest => dest.CityName, src => src.Person != null && src.Person.City != null ? src.Person.City.Name : string.Empty)
               .Map(dest => dest.Email, src => src.Email)
               .Map(dest => dest.Active, src => src.Active)
               .Map(dest => dest.Roles, src => src.RolUsers.Select(ru => ru.Rol.Name));

            config.NewConfig<UserCreateDto, User>()
                .Ignore(dest => dest.Id);

            config.NewConfig<UserCreateDto, Person>()
                .Ignore(dest => dest.Id);

            config.NewConfig<UserUpdateDto, User>();

            config.NewConfig<UserUpdateDto, Person>()
                .Ignore(dest => dest.Id)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Phone, src => src.Phone)
                .Map(dest => dest.Address, src => src.Address)
                .Map(dest => dest.CityId, src => src.CityId)
                .Ignore(dest => dest.Document);
        }
    }
}

