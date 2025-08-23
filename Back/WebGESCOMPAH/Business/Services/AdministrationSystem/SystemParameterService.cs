using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using MapsterMapper;

namespace Business.Services.AdministrationSystem
{
    public class SystemParameterService : BusinessGeneric<SystemParameterSelectDto, SystemParameterDto, SystemParameterUpdateDto, SystemParameter>, ISystemParameterService
    {
        public SystemParameterService(IDataGeneric<SystemParameter> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
