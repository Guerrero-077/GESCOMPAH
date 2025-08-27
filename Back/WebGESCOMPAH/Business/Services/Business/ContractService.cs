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
using Utilities.Exceptions;
using Utilities.Helpers.GeneratePassword;
using Utilities.Messaging.Interfaces;

namespace Business.Services.Business
{
    public class ContractService : BusinessGeneric<ContractSelectDto, ContractCreateDto, ContractUpdateDto, Contract>, IContractService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IPremisesLeasedRepository _premisesLeasedRepository;
        private readonly IContractTermsRepository _contractTermsRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISendCode _emailService;
        private readonly ApplicationDbContext _context;

        public ContractService(

            IContractRepository data,
            IPersonRepository personRepository,
            IUserRepository userRepository,
            IRolUserRepository rolUserRepository,
            IPremisesLeasedRepository premisesLeasedRepository,
            IContractTermsRepository contractTermsRepository,
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
            _contractTermsRepository = contractTermsRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _context = context;
        }


        public async Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            // 1. Verificar o crear la persona
            var person = await _personRepository.GetByDocumentAsync(dto.Document);
            if (person is null)
            {
                person = _mapper.Map<Person>(dto);
                await _personRepository.AddAsync(person);
            }

            // 2. Verificar si ya tiene un usuario
            var user = await _userRepository.GetByPersonIdAsync(person.Id);

            // 3. Crear usuario si no existe y hay email
            if (user is null && !string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _userRepository.ExistsByEmailAsync(dto.Email))
                    throw new BusinessException("El correo ya está registrado.");

                var tempPassword = PasswordGenerator.Generate(12);

                user = new User
                {
                    Email = dto.Email,
                    PersonId = person.Id,
                    Password = _passwordHasher.HashPassword(null!, tempPassword),
                    //FirstLogin = true 
                };

                await _userRepository.AddAsync(user);
                await _rolUserRepository.AsignateRolDefault(user);

                var fullName = $"{person.FirstName} {person.LastName}";
                await _emailService.SendTemporaryPasswordAsync(user.Email, fullName, tempPassword);
            }

            // 4. Crear contrato
            var contract = _mapper.Map<Contract>(dto);
            contract.PersonId = person.Id;
            //contract.Status = ContractStatus.Active;

            await _contractRepository.AddAsync(contract);

            // 5. Asociar locales al contrato
            var premises = dto.EstablishmentIds
                .Select(estId => new PremisesLeased
                {
                    ContractId = contract.Id,
                    EstablishmentId = estId
                });

            await _premisesLeasedRepository.AddRangeAsync(premises);

            // 6. Registrar términos del contrato
            var terms = _mapper.Map<ContractTerms>(dto);
            terms.ContractId = contract.Id;

            await _contractTermsRepository.AddAsync(terms);

            await tx.CommitAsync();

            return contract.Id;
        }



    }
}
