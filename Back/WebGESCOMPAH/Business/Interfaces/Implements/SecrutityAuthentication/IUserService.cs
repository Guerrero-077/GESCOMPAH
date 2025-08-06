using Business.Interfaces.IBusiness;
using Data.Interfaz.IDataImplemenent;
using Entity.DTOs.Implements.SecurityAuthentication.User;

namespace Business.Interfaces.Implements.SecrutityAuthentication
{
    public interface IUserService : IBusiness<UserSelectDto, UserCreateDto, UserUpdateDto>
    {
    }
}
