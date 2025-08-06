using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;

namespace Business.Interfaces.Implements.AdministrationSystem
{
    public interface IFormMouduleService : IBusiness<FormModuleSelectDto, FormModuleCreateDto, FormModuleUpdateDto>
    {
    }
}
