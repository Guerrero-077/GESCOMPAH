using Business.Interfaces.Implements;
using MediatR;

namespace Business.CQRS.Business.Establishments.Delete
{
    public class DeleteEstablishmentHandler : IRequestHandler<DeleteEstablishmentCommand, Unit>
    {
        private readonly IEstablishmentService _service;

        public DeleteEstablishmentHandler(IEstablishmentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DeleteEstablishmentCommand request, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(request.Id, request.ForceDelete);
            return Unit.Value;
        }
    }


}
