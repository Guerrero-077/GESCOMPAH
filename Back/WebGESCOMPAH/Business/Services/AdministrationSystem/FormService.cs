using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using MapsterMapper;

namespace Business.Services.AdministrationSystem
{
    public class FormService : BusinessGeneric<FormSelectDto, FormCreateDto, FormUpdateDto, Form>, IFormService 
    {
        public FormService(IDataGeneric<Form> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
