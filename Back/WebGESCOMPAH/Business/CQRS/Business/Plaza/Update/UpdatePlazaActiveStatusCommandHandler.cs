using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Update
{
    public class UpdatePlazaActiveStatusCommandHandler
        : IRequestHandler<UpdatePlazaActiveStatusCommand, PlazaSelectDto>
    {
        private readonly IPlazaService _plazaService;

        public UpdatePlazaActiveStatusCommandHandler(IPlazaService plazaService)
        {
            _plazaService = plazaService;
        }

        public async Task<PlazaSelectDto> Handle(UpdatePlazaActiveStatusCommand request, CancellationToken cancellationToken)
        {
            return await _plazaService.UpdateActiveStatusAsync(request.Id, request.Active);
        }
    }

}
