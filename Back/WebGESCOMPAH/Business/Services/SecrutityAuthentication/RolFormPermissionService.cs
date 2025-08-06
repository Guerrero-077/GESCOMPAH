using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using MapsterMapper;

namespace Business.Services.SecrutityAuthentication
{
    public class RolFormPermissionService : BusinessGeneric<RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto, RolFormPermission>, IRolFormPermissionService
    {
        public RolFormPermissionService(IRolFormPermissionRepository data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
