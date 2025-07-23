using Entity.DTOs.Implements.Persons.Peron;
using MediatR;

namespace Business.CQRS.Auth.Query.Persons
{
    public class GetAllPersonsQuery : IRequest<IEnumerable<PersonSelectDto>>
    {
        // Si en el futuro quieres aplicar filtros, puedes agregarlos aquí como propiedades.
    }

}
