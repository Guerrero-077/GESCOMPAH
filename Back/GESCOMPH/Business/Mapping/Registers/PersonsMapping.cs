using Entity.Domain.Models.Implements.Persons;

using Entity.DTOs.Implements.Persons.Person;

using Mapster;

namespace Business.Mapping.Registers
{
    public class PersonsMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Person, PersonSelectDto>()
                .Map(dest => dest.CityName, src => src.City != null ? src.City.Name : string.Empty)
                .Map(dest => dest.Email, src => src.User != null ? src.User.Email : null);

            config.NewConfig<PersonDto, Person>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.City)
                .IgnoreNullValues(false);

            config.NewConfig<PersonUpdateDto, Person>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.City)
                .IgnoreNullValues(true);
        }
    }
}

