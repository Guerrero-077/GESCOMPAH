using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.AdministrationSystem.Form;

namespace Business.Interfaces.Implements.AdministrationSystem
{
    public interface IFormService : IBusiness<FormSelectDto, FormCreateDto,  FormUpdateDto>
    {
    }
}
