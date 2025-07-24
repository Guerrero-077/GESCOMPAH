using MediatR;

namespace Business.CQRS.Business.Establishments.Delete
{
    public record DeleteEstablishmentCommand(int Id, bool ForceDelete) : IRequest<Unit>;


}
