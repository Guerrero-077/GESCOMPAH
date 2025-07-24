using Business.Interfaces.Implements;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Persons.Peron;
using MapsterMapper;

public class PersonService : BusinessGeneric<PersonDto, PersonSelectDto, PersonUpdateDto, Person>, IPersonService
{
    private readonly IDataGeneric<Person> _personRepository;

    public PersonService(IDataGeneric<Person> data, IMapper mapper, IDataGeneric<Person> personRepository)
        : base(data, mapper)
    {
        _personRepository = personRepository;
    }

    public async Task<IEnumerable<PersonSelectDto>> GetAllPersonAsync()
    {
        try
        {
            var entities = await _personRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PersonSelectDto>>(entities);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las personas", ex);
        }
    }
}
