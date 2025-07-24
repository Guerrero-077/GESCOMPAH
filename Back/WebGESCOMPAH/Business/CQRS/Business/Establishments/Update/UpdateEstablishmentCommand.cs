using Entity.DTOs.Implements.Business.Establishment;
using MediatR;

namespace Business.CQRS.Business.Establishments.Update
{
    public record UpdateEstablishmentCommand(EstablishmentUpdateDto Dto) : IRequest<EstablishmentSelectDto>;

}
