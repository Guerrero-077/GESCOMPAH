using Business.Interfaces.Implements;
using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Select
{
    public class GetPlazaByIdQueryHandler : IRequestHandler<GetPlazaByIdQuery, PlazaSelectDto?>
    {
        private readonly IPlazaService _plazaService;

        public GetPlazaByIdQueryHandler(IPlazaService plazaService)
        {
            _plazaService = plazaService;
        }

        public async Task<PlazaSelectDto?> Handle(GetPlazaByIdQuery request, CancellationToken cancellationToken)
        {
            return await _plazaService.GetByIdAsync(request.Id);
        }
    }
}
