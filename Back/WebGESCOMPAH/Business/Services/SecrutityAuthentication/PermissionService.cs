using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using MapsterMapper;

namespace Business.Services.SecrutityAuthentication
{
    public class PermissionService(IDataGeneric<Permission> data, IMapper mapper) : BusinessGeneric<PermissionSelectDto, PermissionCreateDto, PermissionUpdateDto, Permission>(data, mapper), IPermissionService
    {
    }
}
