using Entity.DTOs.Implements.Business.Establishment;
using MediatR;

namespace Business.CQRS.Business.Establishments.Select
{
    public record GetAllEstablishmentsQuery() : IRequest<IEnumerable<EstablishmentSelectDto>>;

}
