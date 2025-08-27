using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
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
        private readonly IMapper _mapper;

        public RolFormPermissionService(IRolFormPermissionRepository data, IMapper mapper, IUserContextService auth)
            : base(data, mapper)
        {
            _repo = data;
            _auth = auth;
            _mapper = mapper;
        }

        public override async Task<RolFormPermissionSelectDto> CreateAsync(RolFormPermissionCreateDto dto)
        {
            var createdEntities = new List<RolFormPermission>();
            foreach (var permissionId in dto.PermissionIds)
            {
                var entity = new RolFormPermission
                {
                    RolId = dto.RolId,
                    FormId = dto.FormId,
                    PermissionId = permissionId,
                    Active = true
                };
                // We assume the base repository AddAsync adds the entity and saves changes.
                await _repo.AddAsync(entity);
                createdEntities.Add(entity);
            }

            // Invalidate cache for users with this role
            var userIds = await _repo.GetUserIdsByRoleIdAsync(dto.RolId);
            foreach (var uid in userIds) _auth.InvalidateCache(uid);

            var lastEntity = createdEntities.LastOrDefault();
            if (lastEntity == null)
            {
                return null; // Or handle as appropriate
            }

            // Fetch the entity again to load navigation properties for mapping
            var entityToReturn = await _repo.GetByIdAsync(lastEntity.Id);
            return _mapper.Map<RolFormPermissionSelectDto>(entityToReturn);
        }
        public async Task<IEnumerable<RolFormPermissionSelectDto>> GetAllGroupedAsync()
        {
            var allPermissions = await _repo.GetAllAsync();

            var grouped = allPermissions
                .GroupBy(rfp => new { rfp.RolId, rfp.FormId })
                .Select(g =>
                {
                    var first = g.First();
                    return new RolFormPermissionSelectDto                    {
                        RolId = g.Key.RolId,
                        RolName = first.Rol.Name,
                        FormId = g.Key.FormId,
                        FormName = first.Form.Name,
                        Permissions = g.Select(p => new PermissionInfoDto
                        {
                            PermissionId = p.Permission.Id,
                            PermissionName = p.Permission.Name
                        }).ToList(),
                        Active = g.Any(p => p.Active) // El grupo está activo si al menos un permiso lo está
                    };
                });

            return grouped;
        }

        public async Task<bool> DeleteByGroupAsync(int rolId, int formId)
        {
            var recordsToDelete = await _repo.GetByRolAndFormAsync(rolId, formId);
            if (recordsToDelete == null || !recordsToDelete.Any())
            {
                return false;
            }

            foreach (var record in recordsToDelete)
            {
                await _repo.DeleteAsync(record.Id);
            }

            // Invalidar caché
            var userIds = await _repo.GetUserIdsByRoleIdAsync(rolId);
            foreach (var uid in userIds)
            {
                _auth.InvalidateCache(uid);
            }

            return true;
        }
        

        public override async Task<RolFormPermissionSelectDto> UpdateAsync(RolFormPermissionUpdateDto dto)
        {
            // Get all existing permissions for the role and form
            var existingRecords = await _repo.GetByRolAndFormAsync(dto.RolId, dto.FormId);

            // Delete existing records
            foreach (var record in existingRecords)
            {
                await _repo.DeleteAsync(record.Id); // Assumes soft delete
            }

            // Add the new set of permissions
            var createdEntities = new List<RolFormPermission>();
            foreach (var permissionId in dto.PermissionIds)
            {
                var entity = new RolFormPermission
                {
                    RolId = dto.RolId,
                    FormId = dto.FormId,
                    PermissionId = permissionId
                };
                await _repo.AddAsync(entity);
                createdEntities.Add(entity);
            }

            // Invalidate cache
            var userIds = await _repo.GetUserIdsByRoleIdAsync(dto.RolId);
            foreach (var uid in userIds) _auth.InvalidateCache(uid);

            var lastEntity = createdEntities.LastOrDefault();
            if (lastEntity == null)
            {
                return null; // Or handle as appropriate
            }

            // Fetch one of the new records to return
            var entityToReturn = await _repo.GetByIdAsync(lastEntity.Id);
            return _mapper.Map<RolFormPermissionSelectDto>(entityToReturn);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            // This method remains unchanged as it deletes a single permission record
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