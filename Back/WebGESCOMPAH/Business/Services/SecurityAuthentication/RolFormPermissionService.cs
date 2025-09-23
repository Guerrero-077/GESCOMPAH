using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.SecurityAuthentication
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
            // 1. Validar duplicados antes de crear
            await ValidateDuplicatesAsync(dto.RolId, dto.FormId, dto.PermissionIds);

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

                await _repo.AddAsync(entity);
                createdEntities.Add(entity);
            }

            await InvalidateUsersByRole(dto.RolId);

            if (createdEntities.Count == 0)
                return null;

            var entityToReturn = await _repo.GetByIdAsync(createdEntities.Last().Id);
            return _mapper.Map<RolFormPermissionSelectDto>(entityToReturn);
        }

        public override async Task<RolFormPermissionSelectDto?> UpdateAsync(RolFormPermissionUpdateDto dto)
        {
            // 1. Traer el grupo actual (solo los activos/no borrados)
            var group = (await _repo.GetByRolAndFormAsync(dto.RolId, dto.FormId)).ToList();

            // 2. Normalizar listas
            var existingPermissionIds = group.Select(g => g.PermissionId).ToHashSet();
            var incomingPermissionIds = dto.PermissionIds.Distinct().ToHashSet();

            // 3. Calcular diferencias
            var toAdd = incomingPermissionIds.Except(existingPermissionIds).ToList();
            var toRemove = group.Where(g => !incomingPermissionIds.Contains(g.PermissionId)).ToList();
            var toKeep = group.Where(g => incomingPermissionIds.Contains(g.PermissionId)).ToList();

            // 4. Validar duplicados solo para los nuevos permisos
            if (toAdd.Any())
                await ValidateDuplicatesAsync(dto.RolId, dto.FormId, toAdd);

            // 5. Eliminar los que sobran (solo esos, no todo el grupo)
            foreach (var item in toRemove)
                await _repo.DeleteAsync(item.Id);

            // 6. Agregar los que faltan
            foreach (var pid in toAdd)
            {
                var entity = new RolFormPermission
                {
                    RolId = dto.RolId,
                    FormId = dto.FormId,
                    PermissionId = pid,
                    Active = true
                };
                await _repo.AddAsync(entity);
                group.Add(entity); // Actualizar snapshot local
            }

            await InvalidateUsersByRole(dto.RolId);

            // 7. Armar el DTO agrupado final
            if (group.Count == 0)
                return null;

            var dtoOut = new RolFormPermissionSelectDto
            {
                Id = group.First().Id,
                RolId = group.First().RolId,
                RolName = group.First().Rol?.Name ?? "",
                FormId = group.First().FormId,
                FormName = group.First().Form?.Name ?? "",
                Permissions = group
                    .Select(g => new PermissionInfoDto
                    {
                        PermissionId = g.PermissionId,
                        PermissionName = g.Permission?.Name ?? ""
                    })
                    .ToList(),
                Active = group.Any(g => g.Active)
            };

            return dtoOut;
        }

        public async Task<IEnumerable<RolFormPermissionSelectDto>> GetAllGroupedAsync()
        {
            var all = await _repo.GetAllAsync();

            var grouped = all
                .GroupBy(x => new { x.RolId, x.FormId })
                .Select(g =>
                {
                    var first = g.First();
                    return new RolFormPermissionSelectDto
                    {
                        Id = first.Id,
                        RolId = g.Key.RolId,
                        RolName = first.Rol?.Name ?? "",
                        FormId = g.Key.FormId,
                        FormName = first.Form?.Name ?? "",
                        Permissions = g.Select(p => new PermissionInfoDto
                        {
                            PermissionId = p.PermissionId,
                            PermissionName = p.Permission?.Name ?? ""
                        }).ToList(),
                        Active = g.Any(p => p.Active)
                    };
                });

            return grouped;
        }

        public async Task<bool> DeleteByGroupAsync(int rolId, int formId)
        {
            var records = await _repo.GetByRolAndFormAsync(rolId, formId);
            if (!records.Any()) return false;

            foreach (var record in records)
                await _repo.DeleteAsync(record.Id);

            await InvalidateUsersByRole(rolId);
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var rfp = await _repo.GetByIdAsync(id);
            var result = await base.DeleteAsync(id);

            if (result && rfp != null)
                await InvalidateUsersByRole(rfp.RolId);

            return result;
        }

        private async Task InvalidateUsersByRole(int rolId)
        {
            var userIds = await _repo.GetUserIdsByRoleIdAsync(rolId);
            foreach (var uid in userIds)
                _auth.InvalidateCache(uid);
        }

        public override async Task<RolFormPermissionSelectDto> UpdateActiveStatusAsync(int id, bool active)
        {
            var target = await _repo.GetByIdAsync(id);
            if (target == null)
                throw new KeyNotFoundException($"No se encontró el permiso con ID {id}");

            // 1. Obtener el grupo completo
            var group = await _repo.GetByRolAndFormAsync(target.RolId, target.FormId);

            // 2. Actualizar el estado de todos los elementos en el grupo
            foreach (var item in group)
            {
                if (item.Active != active)
                {
                    item.Active = active;
                    await _repo.UpdateAsync(item);
                }
            }

            await InvalidateUsersByRole(target.RolId);

            // 3. Devolver el DTO de uno de los elementos (todos tienen el mismo Rol y Form)
            var refreshed = await _repo.GetByIdAsync(id);
            return _mapper.Map<RolFormPermissionSelectDto>(refreshed);
        }

        private async Task ValidateDuplicatesAsync(int rolId, int formId, IEnumerable<int> permissionIds)
        {
            var existing = await _repo.GetByRolAndFormAsync(rolId, formId);

            foreach (var pid in permissionIds)
            {
                var existingPerm = existing.FirstOrDefault(p => p.PermissionId == pid);
                if (existingPerm != null)
                {
                    // Verificar si ya existe un permiso con el mismo ID
                    throw new InvalidOperationException($"El permiso con ID {pid} ya existe en este formulario.");
                }
            }
        }


        protected override Expression<Func<RolFormPermission, string?>>[] SearchableFields() =>
        [
            x => x.Rol.Name,
            x => x.Form.Name,
            x => x.Permission.Name
        ];

        protected override string[] SortableFields() =>
        [
            nameof(RolFormPermission.Id),
            nameof(RolFormPermission.RolId),
            nameof(RolFormPermission.FormId),
            nameof(RolFormPermission.PermissionId),
            nameof(RolFormPermission.CreatedAt),
            nameof(RolFormPermission.Active)
        ];

        protected override IDictionary<string, Func<string, Expression<Func<RolFormPermission, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<RolFormPermission, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(RolFormPermission.RolId)] = val => x => x.RolId == int.Parse(val),
                [nameof(RolFormPermission.FormId)] = val => x => x.FormId == int.Parse(val),
                [nameof(RolFormPermission.PermissionId)] = val => x => x.PermissionId == int.Parse(val),
                [nameof(RolFormPermission.Active)] = val => x => x.Active == bool.Parse(val)
            };

    }
}
