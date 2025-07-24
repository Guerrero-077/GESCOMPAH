using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Establishment;
using MediatR;

namespace Business.CQRS.Business.Establishments.Create
{
    public class CreateEstablishmentHandler : IRequestHandler<CreateEstablishmentCommand, EstablishmentSelectDto> 
    {
        private readonly IEstablishmentService _service;

        public CreateEstablishmentHandler(IEstablishmentService service)
        {
            _service = service;
        }

        public async Task<EstablishmentSelectDto> Handle(CreateEstablishmentCommand request, CancellationToken ct)
        {
            return await _service.CreateAsync(request.Dto);
        }
    }

}
