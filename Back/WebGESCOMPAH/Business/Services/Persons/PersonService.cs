using Business.Interfaces.Implements.Persons;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.Persons;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Persons.Peron;
using MapsterMapper;
using Utilities.Exceptions;

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
            throw new BusinessException("Payload inválido.");

        // Validar documento duplicado (regla de negocio propia de Persona)
        if (await _personRepository.ExistsByDocumentAsync(dto.Document))
            throw new BusinessException("Ya existe una persona registrada con ese número de documento.");

        // Mapear DTO -> Entidad
        var entity = _mapper.Map<Person>(dto);

        // Persistir en base de datos
        var created = await Data.AddAsync(entity);

        // Recargar con navegación (City, si aplica)
        var reloaded = await _personRepository.GetByIdWithCityAsync(created.Id) ?? created;

        // Mapear Entidad -> SelectDto
        return _mapper.Map<PersonSelectDto>(reloaded);
    }

    public override async Task<PersonSelectDto> UpdateAsync(PersonUpdateDto dto)
    {
        if (dto == null)
            throw new BusinessException("Payload inválido.");

        var existing = await Data.GetByIdAsync(dto.Id)
            ?? throw new BusinessException("Persona no encontrada.");

        // Si cambia el documento, validar que el nuevo no esté duplicado
        if (!string.Equals(existing.Document, dto.Document, StringComparison.OrdinalIgnoreCase)
            && await _personRepository.ExistsByDocumentAsync(dto.Document))
        {
            throw new BusinessException("Ya existe otra persona con ese número de documento.");
        }

        // Mapear los nuevos valores al entity existente
        _mapper.Map(dto, existing); // Respeta IgnoreNullValues si lo configuraste en Mapster

        await Data.UpdateAsync(existing);

        var reloaded = await _personRepository.GetByIdWithCityAsync(existing.Id) ?? existing;

        return _mapper.Map<PersonSelectDto>(reloaded);
    }
}
