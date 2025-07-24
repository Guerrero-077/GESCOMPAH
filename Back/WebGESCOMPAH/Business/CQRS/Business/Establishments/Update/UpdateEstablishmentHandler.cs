using Business.Interfaces.Implements;
using Entity.DTOs.Implements.Business.Establishment;
using MediatR;

namespace Business.CQRS.Business.Establishments.Update
{
    public class UpdateEstablishmentHandler : IRequestHandler<UpdateEstablishmentCommand, EstablishmentSelectDto>
    {
        private readonly IEstablishmentService _service;

        public UpdateEstablishmentHandler(IEstablishmentService service)
        {
            _service = service;
        }

        public async Task<EstablishmentSelectDto> Handle(UpdateEstablishmentCommand request, CancellationToken ct)
        {
            return await _service.UpdateAsync(request.Dto);
        }
    }
}
