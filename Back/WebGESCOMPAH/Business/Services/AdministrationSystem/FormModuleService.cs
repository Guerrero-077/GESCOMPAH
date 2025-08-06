using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.AdministrationSystem;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using MapsterMapper;

namespace Business.Services.AdministrationSystem
{
    public class FormModuleService : BusinessGeneric<FormModuleSelectDto, FormModuleCreateDto, FormModuleUpdateDto, FormModule>, IFormMouduleService
    {
        public FormModuleService(IFormModuleRepository data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
