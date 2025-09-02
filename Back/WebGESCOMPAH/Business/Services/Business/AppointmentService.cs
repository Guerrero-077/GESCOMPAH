using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Persons;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Business.Appointment;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Utilities.Exceptions;
using Utilities.Helpers.Business;

namespace Business.Services.Business
{
    public class AppointmentService : BusinessGeneric<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto, Appointment>, IAppointmentService
    {
        private readonly IAppointmentRepository _data;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _dataPerson;

        public AppointmentService(
            IAppointmentRepository data, 
            IMapper mapper, 
            IPersonRepository dataPerson
        )
            :base (data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _dataPerson = dataPerson;
        }

        public async Task<bool> RejectedAppointment(int id)
        {
            BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");
            return await _data.RejectedAppointment(id);
        }

        public override async Task<AppointmentSelectDto> CreateAsync(AppointmentCreateDto dto) 
        {
            Person person;

            try
            {
                BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");

                var existPerson = await _dataPerson.GetByDocumentAsync(dto.Document);

                if (existPerson != null)
                {
                    if (existPerson.FirstName != dto.FirstName ||
                        existPerson.LastName != dto.LastName ||
                        existPerson.CityId != dto.cityId)
                    {
                        throw new BusinessException($"Ya existe una persona con documento {dto.Document} pero con datos diferentes. Verifica la información.");
                    }

                    // Si la persona ya existe y coincide la información, creamos la cita con ese ID
                    var existingAppointment = new Appointment
                    {
                        Description = dto.Description,
                        RequestDate = dto.RequestDate,
                        DateTimeAssigned = dto.DateTimeAssigned,
                        EstablishmentId = dto.EstablishmentId,
                        PersonId = existPerson.Id
                    };

                    var existingAppointmentCreated = await _data.AddAsync(existingAppointment);
                    return _mapper.Map<AppointmentSelectDto>(existingAppointmentCreated);
                }

                person = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Document = dto.Document,
                    Address = dto.Address,
                    Phone = dto.Phone,
                    CityId = dto.cityId
                };

                var personCreate = await _dataPerson.AddAsync(person);

                var newAppointment = new Appointment
                {
                    Description = dto.Description,
                    RequestDate = dto.RequestDate,
                    DateTimeAssigned = dto.DateTimeAssigned,
                    EstablishmentId = dto.EstablishmentId,
                    PersonId = personCreate.Id
                };

                var newAppointmentCreated = await _data.AddAsync(newAppointment);

                return _mapper.Map<AppointmentSelectDto>(newAppointmentCreated);
            }
            catch (DbUpdateException dbx)
            {
                // Si hay un índice único en BD, traducimos a excepción de negocio más clara
                throw new BusinessException("Violación de unicidad al crear el registro. Revisa valores únicos.", dbx);
            }

        }
    }
}
