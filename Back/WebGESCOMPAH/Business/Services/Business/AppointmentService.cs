using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Business.Services.SecurityAuthentication;
using CloudinaryDotNet.Actions;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Persons;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.Business.Appointment;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Utilities.Exceptions;
using Utilities.Helpers.Business;
using Utilities.Messaging.Interfaces;
using System.Linq.Expressions;

namespace Business.Services.Business
{
    public class AppointmentService : BusinessGeneric<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto, Appointment>, IAppointmentService
    {
        private readonly IAppointmentRepository _data;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _dataPerson;
        private readonly IUserService _userRepository;
        private readonly ISendCode _emailService;

        public AppointmentService(
            IAppointmentRepository data, 
            IMapper mapper, 
            IPersonRepository dataPerson,
            IUserService userRepository,
            ISendCode emailService
        )
            :base (data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _dataPerson = dataPerson;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public override async Task<AppointmentSelectDto> CreateAsync(AppointmentCreateDto dto) 
        {

            try
            {
                BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");

                var existPerson = await _dataPerson.GetByDocumentAsync(dto.Document);

                if (existPerson != null)
                {
                    if (existPerson.FirstName != dto.FirstName ||
                        existPerson.LastName != dto.LastName ||
                        existPerson.CityId != dto.CityId)
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
                        PersonId = existPerson.Id,
                        Active = true,
                    };

                    var existingAppointmentCreated = await _data.AddAsync(existingAppointment);
                    return _mapper.Map<AppointmentSelectDto>(existingAppointmentCreated);
                }
                
                var newPerson = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Document = dto.Document,
                    Address = dto.Address,
                    Phone = dto.Phone,
                    CityId = dto.CityId,
                };

                var personCreate = await _dataPerson.AddAsync(newPerson);

                var userCreate = await _userRepository.EnsureUserForPersonAsync(personCreate.Id, dto.Email);
                await _emailService.SendTemporaryPasswordAsync(dto.Email, dto.FirstName, userCreate.tempPassword);

                var newAppointment = new Appointment  
                {
                    Description = dto.Description,
                    RequestDate = dto.RequestDate,
                    DateTimeAssigned = dto.DateTimeAssigned,
                    EstablishmentId = dto.EstablishmentId,
                    PersonId = personCreate.Id,
                    Active = true,
                };

                var appoitmentCreate = await _data.AddAsync(newAppointment);

                return _mapper.Map<AppointmentSelectDto>(newAppointment);
            }
            catch (DbUpdateException dbx)
            {
                // Si hay un índice único en BD, traducimos a excepción de negocio más clara
                throw new BusinessException("Violación de unicidad al crear el registro. Revisa valores únicos.", dbx);
            }

        }

        protected override Expression<Func<Appointment, string>>[] SearchableFields() =>
            [
                a => a.Description!,
                a => a.Person.FirstName!,
                a => a.Person.LastName!,
                a => a.Person.Phone!,
                a => a.Establishment.Name!
            ];

        protected override string[] SortableFields() => new[]
        {
            nameof(Appointment.Description),
            nameof(Appointment.RequestDate),
            nameof(Appointment.DateTimeAssigned),
            nameof(Appointment.EstablishmentId),
            nameof(Appointment.PersonId),
            nameof(Appointment.Id),
            nameof(Appointment.CreatedAt),
            nameof(Appointment.Active)
        };

        protected override IDictionary<string, Func<string, Expression<Func<Appointment, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<Appointment, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Appointment.EstablishmentId)] = v => e => e.EstablishmentId == int.Parse(v),
                [nameof(Appointment.PersonId)] = v => e => e.PersonId == int.Parse(v),
                [nameof(Appointment.Active)] = v => e => e.Active == bool.Parse(v),
                [nameof(Appointment.RequestDate)] = v => e => e.RequestDate == DateTime.Parse(v)
            };

    }
}

