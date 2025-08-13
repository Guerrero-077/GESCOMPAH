using Business.Interfaces.Implements.Persons;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.Location;
using Data.Interfaz.IDataImplemenent.Persons;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Persons.Peron;
using MapsterMapper;
using Utilities.Exceptions;

public class PersonService : BusinessGeneric<PersonSelectDto,PersonDto, PersonUpdateDto, Person>, IPersonService
{
    private readonly IPersonRepository _personRepository;
    public PersonService(IPersonRepository data, IMapper mapper)
        : base(data, mapper)
    {
        _personRepository = data;
    }

    public override async Task<PersonSelectDto> CreateAsync(PersonDto dto)
    {
        if (dto == null) throw new BusinessException("Payload inválido.");

        // DTO -> Entidad
        var entity = _mapper.Map<Person>(dto);

        // Persistir
        var created = await Data.AddAsync(entity);

        // Recargar con City para mapear CityName sin NRE
        var reloaded = await _personRepository.GetByIdWithCityAsync(created.Id) ?? created;

        // Entidad -> SelectDto
        return _mapper.Map<PersonSelectDto>(reloaded);
    }

    public override async Task<PersonSelectDto> UpdateAsync(PersonUpdateDto dto)
    {
        if (dto == null) throw new BusinessException("Payload inválido.");

        var entity = await Data.GetByIdAsync(dto.Id)
            ?? throw new BusinessException("Persona no encontrada.");

        // Aplicar cambios (patch-friendly por IgnoreNullValues(true))
        _mapper.Map(dto, entity);

        await Data.UpdateAsync(entity);

        var reloaded = await _personRepository.GetByIdWithCityAsync(entity.Id) ?? entity;

        return _mapper.Map<PersonSelectDto>(reloaded);
    }

}
