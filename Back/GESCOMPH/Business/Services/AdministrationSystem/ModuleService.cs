using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using MapsterMapper;

namespace Business.Services.AdministrationSystem
{
    public class ModuleService : BusinessGeneric<ModuleSelectDto, ModuleCreateDto, ModuleUpdateDto, Module>, IModuleService
    {
        public ModuleService(IDataGeneric<Module> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
