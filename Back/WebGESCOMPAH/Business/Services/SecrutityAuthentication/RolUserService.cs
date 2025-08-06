using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using MapsterMapper;

namespace Business.Services.SecrutityAuthentication
{
    public class RolUserService : BusinessGeneric<RolUserSelectDto, RolUserCreateDto, RolUserUpdateDto, RolUser>, IRolUserService
    {
        public RolUserService(IRolUserRepository data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
