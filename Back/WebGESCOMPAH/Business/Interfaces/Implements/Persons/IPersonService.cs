using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Persons.Person;

namespace Business.Interfaces.Implements.Persons
{
    public interface IPersonService : IBusiness<PersonSelectDto, PersonDto, PersonUpdateDto>
    {
        Task<PersonSelectDto?> GetByDocumentAsync(string document);
        Task<PersonSelectDto> GetOrCreateByDocumentAsync(PersonDto dto);
    }
}
