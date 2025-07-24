using Business.CQRS.Persons.Person.Select;
using Business.Interfaces.Implements;
using Entity.DTOs.Implements.Persons.Peron;
using MediatR;

public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, IEnumerable<PersonSelectDto>>
{
    private readonly IPersonService _personService;

    public GetAllPersonsQueryHandler(IPersonService personService)
    {
        _personService = personService;
    }

    public async Task<IEnumerable<PersonSelectDto>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
    {
        return await _personService.GetAllPersonAsync();
    }
}
