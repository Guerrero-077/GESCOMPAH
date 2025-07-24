using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MapsterMapper;

namespace Business.Services.SecrutityAuthentication
{
    public class RolService : BusinessGeneric<RolSelectDto, RolDto, RolDto, Rol>, IRolService
    {
        private readonly IDataGeneric<Rol> _rolRepository;
        public RolService(IDataGeneric<Rol> rolRepository, IMapper mapper) : base(rolRepository, mapper)
        {
            _rolRepository = rolRepository;
        }

    }
}
