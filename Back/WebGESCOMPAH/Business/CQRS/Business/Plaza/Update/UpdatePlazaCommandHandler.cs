using Business.Interfaces.Implements;
using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Update
{
    public class UpdatePlazaCommandHandler : IRequestHandler<UpdatePlazaCommand, PlazaSelectDto>
    {
        private readonly IPlazaService _plazaService;

        public UpdatePlazaCommandHandler(IPlazaService plazaService)
        {
            _plazaService = plazaService;
        }

        public async Task<PlazaSelectDto> Handle(UpdatePlazaCommand request, CancellationToken cancellationToken)
        {
            return await _plazaService.UpdateAsync(request.Plaza);
        }
    }


}
