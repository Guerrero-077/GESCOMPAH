using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Persons;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.Business.Contract;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Utilities.Exceptions;
using Utilities.Helpers.GeneratePassword;
using Utilities.Messaging.Interfaces;
using System.Linq;

namespace Business.Services.Business
{
    /// <summary>
    /// Crea contratos (persona/usuario opcional), asocia locales y marca sus establecimientos como ocupados (Active = false).
    /// </summary>
    public class ContractService
        : BusinessGeneric<ContractSelectDto, ContractCreateDto, ContractUpdateDto, Contract>, IContractService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IPremisesLeasedRepository _premisesLeasedRepository;
        private readonly IEstablishmentsRepository _establishmentsRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISendCode _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ContractService(
            IContractRepository data,
            IPersonRepository personRepository,
            IUserRepository userRepository,
            IRolUserRepository rolUserRepository,
            IPremisesLeasedRepository premisesLeasedRepository,
            IEstablishmentsRepository establishmentsRepository,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            ISendCode emailService,
            ApplicationDbContext context
        ) : base(data, mapper)
        {
            _personRepository = personRepository;
            _userRepository = userRepository;
            _rolUserRepository = rolUserRepository;
            _contractRepository = data;
            _premisesLeasedRepository = premisesLeasedRepository;
            _establishmentsRepository = establishmentsRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _context = context;
            _mapper = mapper;
        }

        // Proyección liviana -> suma totales sin materializar entidades completas.
        private async Task<(decimal totalBase, decimal totalUvt)> SumEstablishmentsAsync(IEnumerable<int> establishmentIds)
        {
            var ids = establishmentIds?.Distinct().ToList() ?? [];
            if (ids.Count == 0) return (0m, 0m);

            var basics = await _establishmentsRepository.GetBasicsByIdsAsync(ids);
            var totalBase = basics.Sum(b => b.RentValueBase);
            var totalUvt = basics.Sum(b => b.UvtQty);
            return (totalBase, totalUvt);
        }

        public async Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto)
        {
            if (dto.EstablishmentIds is null || dto.EstablishmentIds.Count == 0)
                throw new BusinessException("Debe seleccionar al menos un establecimiento.");

            var targetIds = dto.EstablishmentIds.Distinct().ToList();

            await using var tx = await _context.Database.BeginTransactionAsync();

            // 0) Validación de disponibilidad: todos deben estar activos
            var alreadyInactive = await _establishmentsRepository.GetInactiveIdsAsync(targetIds);
            if (alreadyInactive.Count > 0)
            {
                throw new BusinessException(
                    $"Los establecimientos {string.Join(", ", alreadyInactive)} no están disponibles (Active = false)."
                );
            }

            // 1) Persona (crear o reutilizar por documento)
            var person = await _personRepository.GetByDocumentAsync(dto.Document);
            if (person is null)
            {
                person = _mapper.Map<Person>(dto);
                await _personRepository.AddAsync(person);
                await _context.SaveChangesAsync();
            }

            // 2) Usuario opcional
            var user = await _userRepository.GetByPersonIdAsync(person.Id);
            if (user is null && !string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _userRepository.ExistsByEmailAsync(dto.Email))
                    throw new BusinessException("El correo ya está registrado.");

                var tempPass = PasswordGenerator.Generate(12);
                user = new User
                {
                    Email = dto.Email.Trim(),
                    PersonId = person.Id,
                    Password = _passwordHasher.HashPassword(null!, tempPass)
                };
                await _userRepository.AddAsync(user);
                await _rolUserRepository.AsignateRolDefault(user);

                var fullName = $"{person.FirstName} {person.LastName}";
                await _emailService.SendTemporaryPasswordAsync(user.Email, fullName, tempPass);
                await _context.SaveChangesAsync();
            }

            // 3) Entidad contrato
            var contract = _mapper.Map<Contract>(dto);
            contract.PersonId = person.Id;

            // 4) Calcular totales por proyección
            var (totalBase, totalUvt) = await SumEstablishmentsAsync(targetIds);
            contract.TotalBaseRentAgreed = totalBase;
            contract.TotalUvtQtyAgreed = totalUvt;

            await _contractRepository.AddAsync(contract);
            await _context.SaveChangesAsync(); // genera Contract.Id

            // 5) Asociar locales
            var premises = targetIds.Select(id => new PremisesLeased
            {
                ContractId = contract.Id,
                EstablishmentId = id
            }).ToList();

            await _premisesLeasedRepository.AddRangeAsync(premises);
            await _context.SaveChangesAsync();

            // 6) Marcar establecimientos como ocupados (Active = false) en BLOQUE
            //    Filtro por Active != false para no tocar filas innecesarias.
            var rows = await _establishmentsRepository.SetActiveByIdsAsync(targetIds, active: false);

            if (rows != targetIds.Count)
            {
                // Concurrencia: alguien pudo cambiar estado entre validación y update.
                throw new BusinessException(
                    "Conflicto de concurrencia al actualizar estados de establecimientos. " +
                    "Intente nuevamente; verifique disponibilidad actual."
                );
            }

            await tx.CommitAsync();
            return contract.Id;
        }
    }
}
