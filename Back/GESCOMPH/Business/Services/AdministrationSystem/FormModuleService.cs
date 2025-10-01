using Business.Interfaces.Implements.AdministrationSystem;
using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplement.AdministrationSystem;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.AdministrationSystem
{
    public class FormModuleService
        : BusinessGeneric<FormModuleSelectDto, FormModuleCreateDto, FormModuleUpdateDto, FormModule>,
          IFormMouduleService
    {
        private readonly IFormModuleRepository _repo;
        private readonly IUserContextService _auth;

        public FormModuleService(IFormModuleRepository data, IMapper mapper, IUserContextService auth)
            : base(data, mapper)
        {
            _repo = data;
            _auth = auth;
        }

        // Aquí defines la llave “única” de negocio
        protected override IQueryable<FormModule>? ApplyUniquenessFilter(IQueryable<FormModule> query, FormModule candidate)
            => query.Where(fm => fm.FormId == candidate.FormId && fm.ModuleId == candidate.ModuleId);

        public override async Task<FormModuleSelectDto> CreateAsync(FormModuleCreateDto dto)
        {
            var result = await base.CreateAsync(dto);

            // Invalidar caché de usuarios que tengan permisos sobre ese form
            var userIds = await _repo.GetUserIdsByFormIdAsync(dto.FormId);
            foreach (var uid in userIds) _auth.InvalidateCache(uid);

            return result;
        }

        public override async Task<FormModuleSelectDto> UpdateAsync(FormModuleUpdateDto dto)
        {
            var result = await base.UpdateAsync(dto);

            var userIds = await _repo.GetUserIdsByFormIdAsync(dto.FormId);
            foreach (var uid in userIds) _auth.InvalidateCache(uid);

            return result;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var fm = await _repo.GetByIdAsync(id);
            var ok = await base.DeleteAsync(id);

            if (ok && fm != null)
            {
                var userIds = await _repo.GetUserIdsByFormIdAsync(fm.FormId);
                foreach (var uid in userIds) _auth.InvalidateCache(uid);
            }

            return ok;
        }

        protected override Expression<Func<FormModule, string>>[] SearchableFields() =>
        [
            f => f.Form.Name,
            f => f.Module.Name
        ];


        protected override string[] SortableFields() => new[]
        {
            nameof(FormModule.FormId),
            nameof(FormModule.ModuleId),
            nameof(FormModule.Id),
            nameof(FormModule.CreatedAt),
            nameof(FormModule.Active)
        };

        protected override IDictionary<string, Func<string, Expression<Func<FormModule, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<FormModule, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(FormModule.FormId)] = value => entity => entity.FormId == int.Parse(value),
                [nameof(FormModule.ModuleId)] = value => entity => entity.ModuleId == int.Parse(value),
                [nameof(FormModule.Active)] = value => entity => entity.Active == bool.Parse(value)
            };
    }
}