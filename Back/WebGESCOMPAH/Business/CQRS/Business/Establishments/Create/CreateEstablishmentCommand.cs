using Entity.DTOs.Implements.Business.Establishment;
using MediatR;

namespace Business.CQRS.Business.Establishments.Create
{
    public record CreateEstablishmentCommand(EstablishmentCreateDto Dto): IRequest<EstablishmentSelectDto>;


}
