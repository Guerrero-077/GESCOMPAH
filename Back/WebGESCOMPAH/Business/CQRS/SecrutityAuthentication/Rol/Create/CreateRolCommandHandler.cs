using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.Rol.Create
{
    public class CreateRolCommandHandler : IRequestHandler<CreateRolCommand, RolSelectDto>
    {
        private readonly IRolService _rolService;

        public CreateRolCommandHandler(IRolService rolService)
        {
            _rolService = rolService;   
        }

        public async Task<RolSelectDto> Handle(CreateRolCommand request, CancellationToken cancellationToken)
        {
            return await _rolService.CreateAsync(request.Rol);
        }
    }
}
