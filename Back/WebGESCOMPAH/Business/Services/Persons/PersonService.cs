using Business.Interfaces.Implements.Persons;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Persons.Peron;
using MapsterMapper;

public class PersonService : BusinessGeneric<PersonSelectDto,PersonDto, PersonUpdateDto, Person>, IPersonService
{

    public PersonService(IDataGeneric<Person> data, IMapper mapper)
        : base(data, mapper)
    {
    }

}
