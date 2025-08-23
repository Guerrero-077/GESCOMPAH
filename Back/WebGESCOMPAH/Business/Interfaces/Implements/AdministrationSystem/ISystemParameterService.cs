using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;

namespace Business.Interfaces.Implements.AdministrationSystem
{
    public interface ISystemParameterService : IBusiness<SystemParameterSelectDto, SystemParameterDto, SystemParameterUpdateDto>
    {
    }
}
