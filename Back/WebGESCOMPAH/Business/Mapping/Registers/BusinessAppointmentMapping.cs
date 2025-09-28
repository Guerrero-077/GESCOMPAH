using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;

using Entity.DTOs.Implements.Business.Appointment;

using Mapster;

namespace Business.Mapping.Registers
{
    public class BusinessAppointmentMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Appointment, AppointmentSelectDto>()
                .Map(dest => dest.EstablishmentName, src => src.Establishment.Name)
                .Map(dest => dest.PersonName, src => src.Person.FirstName)
                .Map(dest => dest.Phone, src => src.Person.Phone);

            config.NewConfig<Person, AppointmentCreateDto>()
                .Ignore(dest => dest.Description)
                .Ignore(dest => dest.RequestDate)
                .Ignore(dest => dest.DateTimeAssigned)
                .Ignore(dest => dest.EstablishmentId);

            config.NewConfig<Appointment, AppointmentCreateDto>()
                .Ignore(dest => dest.FirstName)
                .Ignore(dest => dest.LastName)
                .Ignore(dest => dest.Document)
                .Ignore(dest => dest.Address)
                .Ignore(dest => dest.Phone)
                .Ignore(dest => dest.CityId);
        }
    }
}

