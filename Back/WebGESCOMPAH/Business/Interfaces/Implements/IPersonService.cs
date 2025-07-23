using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Persons.Peron;

namespace Business.Interfaces.Implements
{
    public interface IPersonService 
    {
        Task<IEnumerable<PersonSelectDto>> GetAllPersonAsync();
    }
}
