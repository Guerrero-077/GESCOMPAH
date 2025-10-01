using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.AdministrationSystem
{
    public class ModuleService : BusinessGeneric<ModuleSelectDto, ModuleCreateDto, ModuleUpdateDto, Module>, IModuleService
    {
        public ModuleService(IDataGeneric<Module> data, IMapper mapper) : base(data, mapper)
        {
        }

        // Aquí defines la llave “única” de negocio
        protected override IQueryable<Module>? ApplyUniquenessFilter(IQueryable<Module> query, Module candidate)
            => query.Where(m => m.Name == candidate.Name);

        protected override Expression<Func<Module, string>>[] SearchableFields() =>
        [
            m => m.Name!,
            m => m.Description!,
            m => m.Icon!
        ];

        protected override string[] SortableFields() =>
        [
            nameof(Module.Name),
            nameof(Module.Description),
            nameof(Module.Icon),
            nameof(Module.Active),
            nameof(Module.CreatedAt),
            nameof(Module.Id)
        ];

    }
}
