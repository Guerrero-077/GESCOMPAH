using Entity.DTOs.Implements.Persons.Peron;
using MediatR;

namespace Business.CQRS.Persons.Person.Select
{
    public class GetAllPersonsQuery : IRequest<IEnumerable<PersonSelectDto>>
    {
        // Si en el futuro quieres aplicar filtros, puedes agregarlos aquí como propiedades.
    }

}
