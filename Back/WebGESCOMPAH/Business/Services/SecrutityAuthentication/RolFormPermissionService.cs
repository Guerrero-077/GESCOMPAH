using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using MapsterMapper;

namespace Business.Services.SecrutityAuthentication
{
    public class RolFormPermissionService
        : BusinessGeneric<RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto, RolFormPermission>,
          IRolFormPermissionService
    {
        private readonly IRolFormPermissionRepository _repo;
        private readonly IUserContextService _auth;

        public RolFormPermissionService(IRolFormPermissionRepository data, IMapper mapper, IUserContextService auth)
            : base(data, mapper)
        {
            _repo = data;
            _auth = auth;
        }

        public override async Task<RolFormPermissionSelectDto> CreateAsync(RolFormPermissionCreateDto dto)
        {
            var result = await base.CreateAsync(dto);

            // Invalidar caché de los usuarios que tienen ese rol
            var userIds = await _repo.GetUserIdsByRoleIdAsync(dto.RolId);
            foreach (var uid in userIds) _auth.InvalidateCache(uid);

            return result;
        }

        public override async Task<RolFormPermissionSelectDto> UpdateAsync(RolFormPermissionUpdateDto dto)
        {
            var result = await base.UpdateAsync(dto);

            var userIds = await _repo.GetUserIdsByRoleIdAsync(dto.RolId);
            foreach (var uid in userIds) _auth.InvalidateCache(uid);

            return result;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            // Necesitamos el RolId para invalidar
            var rfp = await _repo.GetByIdAsync(id);
            var ok = await base.DeleteAsync(id);

            if (ok && rfp != null)
            {
                var userIds = await _repo.GetUserIdsByRoleIdAsync(rfp.RolId);
                foreach (var uid in userIds) _auth.InvalidateCache(uid);
            }

            return ok;
        }
    }
}
