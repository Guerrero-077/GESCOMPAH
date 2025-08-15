using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MapsterMapper;

namespace Business.Services.SecrutityAuthentication
{
    public class RolService(IDataGeneric<Rol> rolRepository, IMapper mapper) : BusinessGeneric<RolSelectDto, RolCreateDto, RolUpdateDto, Rol>(rolRepository, mapper), IRolService
    {
            // Aquí defines la llave “única” de negocio
        protected override IQueryable<Rol>? ApplyUniquenessFilter(IQueryable<Rol> query, Rol candidate)
            => query.Where(f => f.Name == candidate.Name);
    }
}
