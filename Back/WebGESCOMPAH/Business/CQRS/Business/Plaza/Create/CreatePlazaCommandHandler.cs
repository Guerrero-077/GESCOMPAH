using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Create
{
    public class CreatePlazaCommandHandler : IRequestHandler<CreatePlazaCommand, PlazaSelectDto>
    {
        private readonly IPlazaService _plazaService;

        public CreatePlazaCommandHandler(IPlazaService plazaService)
        {
            _plazaService = plazaService;
        }

        public async Task<PlazaSelectDto> Handle(CreatePlazaCommand request, CancellationToken cancellationToken)
        {

            return await _plazaService.CreateAsync(request.Plaza);
        }
    }
}
