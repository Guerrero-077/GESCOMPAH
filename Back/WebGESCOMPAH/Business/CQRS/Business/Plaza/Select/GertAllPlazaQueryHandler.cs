using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Select
{
    public class GertAllPlazaQueryHandler : IRequestHandler<GetAllPlazasQuery, IEnumerable<PlazaSelectDto>>
    {

        private readonly IPlazaService _plazaService;
        public GertAllPlazaQueryHandler(IPlazaService plazaService)
        {
            _plazaService = plazaService;
        }

        public async Task<IEnumerable<PlazaSelectDto>> Handle(GetAllPlazasQuery request, CancellationToken cancellationToken)
        {
            return await _plazaService.GetAllAsync();
        }
    }
}
