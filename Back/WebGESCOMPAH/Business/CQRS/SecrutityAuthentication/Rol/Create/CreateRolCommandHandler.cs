using Business.Interfaces.Implements;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.Rol.Create
{
    public class CreateRolCommandHandler : IRequestHandler<CreateRolCommand, int>
    {
        private readonly IRolService _rolService;

        public CreateRolCommandHandler(IRolService rolService)
        {
            _rolService = rolService;   
        }

        public async Task<int> Handle(CreateRolCommand request, CancellationToken cancellationToken)
        {
            return await _rolService.CreateRolAsync(request.Rol);
        }
    }
}
