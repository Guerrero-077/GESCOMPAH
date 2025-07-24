using Business.Interfaces.Implements;
using Entity.DTOs.Implements.Business.Establishment;
using MediatR;

namespace Business.CQRS.Business.Establishments.Select
{
    public class GetEstablishmentByIdHandler : IRequestHandler<GetEstablishmentByIdQuery, EstablishmentSelectDto?>
    {
        private readonly IEstablishmentService _service;

        public GetEstablishmentByIdHandler(IEstablishmentService service)
        {
            _service = service;
        }

        public async Task<EstablishmentSelectDto?> Handle(GetEstablishmentByIdQuery request, CancellationToken ct)
        {
            return await _service.GetByIdAsync(request.Id);
        }
    }

}
