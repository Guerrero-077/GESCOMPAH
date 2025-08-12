using Business.Interfaces.Implements.Persons;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.Persons;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Persons.Peron;
using MapsterMapper;

public class PersonService : BusinessGeneric<PersonSelectDto,PersonDto, PersonUpdateDto, Person>, IPersonService
{

    public PersonService(IPersonRepository data, IMapper mapper)
        : base(data, mapper)
    {
    }

}
