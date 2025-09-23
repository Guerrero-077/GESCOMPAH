using Business.Interfaces.Implements.Persons;
using Business.Repository;
using Data.Interfaz.IDataImplement.Persons;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Persons.Person;
using MapsterMapper;
using Utilities.Exceptions;
using System.Linq.Expressions;

public class PersonService : BusinessGeneric<PersonSelectDto, PersonDto, PersonUpdateDto, Person>, IPersonService
{
    private readonly IPersonRepository _personRepository;

    public PersonService(IPersonRepository repository, IMapper mapper)
        : base(repository, mapper)
    {
        _personRepository = repository;
    }

    public override async Task<PersonSelectDto> CreateAsync(PersonDto dto)
    {
        if (dto == null)
            throw new BusinessException("Payload invalido.");

        // Validar documento duplicado (regla de negocio propia de Persona)
        if (await _personRepository.ExistsByDocumentAsync(dto.Document))
            throw new BusinessException("Ya existe una persona registrada con ese numero de documento.");

        // Mapear DTO -> Entidad
        var entity = _mapper.Map<Person>(dto);

        // Persistir en base de datos
        var created = await Data.AddAsync(entity);

        // Recargar con navegacion (City, si aplica)
        var reloaded = await _personRepository.GetByIdAsync(created.Id) ?? created;

        // Mapear Entidad -> SelectDto
        return _mapper.Map<PersonSelectDto>(reloaded);
    }

    public override async Task<PersonSelectDto> UpdateAsync(PersonUpdateDto dto)
    {
        if (dto == null)
            throw new BusinessException("Payload invalido.");

        var existing = await Data.GetByIdAsync(dto.Id)
            ?? throw new BusinessException("Persona no encontrada.");

        // Mapear los nuevos valores al entity existente
        _mapper.Map(dto, existing); // Respeta IgnoreNullValues si lo configuraste en Mapster

        await Data.UpdateAsync(existing);

        var reloaded = await _personRepository.GetByIdAsync(existing.Id) ?? existing;

        return _mapper.Map<PersonSelectDto>(reloaded);
    }


    public async Task<PersonSelectDto?> GetByDocumentAsync(string document)
    {
        var person = await _personRepository.GetByDocumentAsync(document);
        return person is null ? null : _mapper.Map<PersonSelectDto>(person);
    }

    public async Task<PersonSelectDto> GetOrCreateByDocumentAsync(PersonDto dto)
    {
        if (dto == null)
            throw new BusinessException("Payload invalido.");

        var existing = await _personRepository.GetByDocumentAsync(dto.Document);
        if (existing is not null)
            return _mapper.Map<PersonSelectDto>(existing);

        return await CreateAsync(dto);
    }

    protected override Expression<Func<Person, string>>[] SearchableFields() =>
    [
        p => p.FirstName,
        p => p.LastName,
        p => p.Document,
        p => p.Address,
        p => p.Phone,
        p => p.User.Email
    ];

    protected override string[] SortableFields() => new[]
    {
        nameof(Person.FirstName),
        nameof(Person.LastName),
        nameof(Person.Document),
        nameof(Person.CityId),
        nameof(Person.Id),
        nameof(Person.CreatedAt),
        nameof(Person.Active)
    };

    protected override IDictionary<string, Func<string, Expression<Func<Person, bool>>>> AllowedFilters() =>
        new Dictionary<string, Func<string, Expression<Func<Person, bool>>>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(Person.Document)] = value => p => p.Document == value,
            [nameof(Person.CityId)] = value => p => p.CityId == int.Parse(value),
            [nameof(Person.Active)] = value => p => p.Active == bool.Parse(value),
            [nameof(Person.FirstName)] = value => p => p.FirstName == value,
            [nameof(Person.LastName)] = value => p => p.LastName == value
        };

}
