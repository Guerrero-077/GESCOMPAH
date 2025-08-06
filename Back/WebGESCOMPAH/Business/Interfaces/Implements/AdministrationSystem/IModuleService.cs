using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.AdministrationSystem.Module;

namespace Business.Interfaces.Implements.AdministrationSystem
{
    public interface IModuleService : IBusiness<ModuleSelectDto, ModuleCreateDto, ModuleUpdateDto>
    {
    }
}
