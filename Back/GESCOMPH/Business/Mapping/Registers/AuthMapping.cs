using System.Linq;

using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.Implements.Persons;

using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Entity.DTOs.Implements.SecurityAuthentication.User;

using Mapster;

namespace Business.Mapping.Registers
{
    public class AuthMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
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
        }
    }
}

