using Business.Interfaces.Implements;
using Entity.DTOs.Implements.Business.Establishment;
using MediatR;

namespace Business.CQRS.Business.Establishments.Select
{
    public class GetAllEstablishmentsHandler : IRequestHandler<GetAllEstablishmentsQuery, IEnumerable<EstablishmentSelectDto>>
    {
        private readonly IEstablishmentService _service;

        public GetAllEstablishmentsHandler(IEstablishmentService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<EstablishmentSelectDto>> Handle(GetAllEstablishmentsQuery request, CancellationToken ct)
        {
            return await _service.GetAllAsync();
        }

    }

}
