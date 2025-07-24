using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Persons.Peron;

namespace Business.Interfaces.Implements.Persons
{
    public interface IPersonService : IBusiness<PersonSelectDto, PersonDto, PersonUpdateDto>
    {
    }
}
