using Business.Interfaces.Implements;
using MediatR;

namespace Business.CQRS.Auth.Commands.Rol
{
    public class CreatePersonCommandHandler : IRequestHandler<CreateRolCommand, int>
    {
        private readonly IRolService _rolService;

        public CreatePersonCommandHandler(IRolService rolService)
        {
            _rolService = rolService;   
        }

        public async Task<int> Handle(CreateRolCommand request, CancellationToken cancellationToken)
        {
            return await _rolService.CreateRolAsync(request.Rol);
        }
    }
}
