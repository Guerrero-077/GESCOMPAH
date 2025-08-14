using Business.Interfaces.Implements.AdministrationSystem;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.AdministrationSystem;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using MapsterMapper;

namespace Business.Services.AdministrationSystem
{
    public class FormModuleService
        : BusinessGeneric<FormModuleSelectDto, FormModuleCreateDto, FormModuleUpdateDto, FormModule>,
          IFormMouduleService
    {
        private readonly IFormModuleRepository _repo;
        private readonly IAuthService _auth;

        public FormModuleService(IFormModuleRepository data, IMapper mapper, IAuthService auth)
            : base(data, mapper)
        {
            _repo = data;
            _auth = auth;
        }

        // Aquí defines la llave “única” de negocio
        protected override IQueryable<FormModule>? ApplyUniquenessFilter(IQueryable<FormModule> query, FormModule candidate)
            => query.Where(f => f.FormId == candidate.FormId);

        public override async Task<FormModuleSelectDto> CreateAsync(FormModuleCreateDto dto)
        {
            var result = await base.CreateAsync(dto);

            // Invalidar caché de usuarios que tengan permisos sobre ese form
            var userIds = await _repo.GetUserIdsByFormIdAsync(dto.FormId);
            foreach (var uid in userIds) _auth.InvalidateUserCache(uid);

            return result;
        }

        public override async Task<FormModuleSelectDto> UpdateAsync(FormModuleUpdateDto dto)
        {
            var result = await base.UpdateAsync(dto);

            var userIds = await _repo.GetUserIdsByFormIdAsync(dto.FormId);
            foreach (var uid in userIds) _auth.InvalidateUserCache(uid);

            return result;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var fm = await _repo.GetByIdAsync(id);
            var ok = await base.DeleteAsync(id);

            if (ok && fm != null)
            {
                var userIds = await _repo.GetUserIdsByFormIdAsync(fm.FormId);
                foreach (var uid in userIds) _auth.InvalidateUserCache(uid);
            }

            return ok;
        }
    }
}
