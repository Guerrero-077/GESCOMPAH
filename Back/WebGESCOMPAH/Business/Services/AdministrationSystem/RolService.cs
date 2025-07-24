using Business.Interfaces.Implements;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MapsterMapper;

namespace Business.Services.AdministrationSystem
{
    public class RolService : BusinessGeneric<RolDto, RolSelectDto, RolSelectDto, Rol>, IRolService
    {
        private readonly IDataGeneric<Rol> _rolRepository;
        public RolService(IDataGeneric<Rol> rolRepository, IMapper mapper) : base(rolRepository, mapper)
        {
            _rolRepository = rolRepository;
        }
        public async Task<int> CreateRolAsync(RolDto rolDto)
        {
            try
            {
                var entity = _mapper.Map<Rol>(rolDto);
                await _rolRepository.AddAsync(entity);

                return entity.Id; // Devuelve el ID generado
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la persona", ex);
            }
        }

    }
}
