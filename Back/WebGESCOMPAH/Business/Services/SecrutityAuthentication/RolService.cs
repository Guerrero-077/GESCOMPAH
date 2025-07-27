using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MapsterMapper;

namespace Business.Services.SecrutityAuthentication
{
    public class RolService(IDataGeneric<Rol> rolRepository, IMapper mapper) : BusinessGeneric<RolSelectDto, RolCreateDto, RolUpdateDto, Rol>(rolRepository, mapper), IRolService
    {
    }
}
