using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.Rol.Create
{
    public class CreateRolCommand : IRequest<RolSelectDto> 
    {
        public RolDto Rol { get; set; }

        public CreateRolCommand(RolDto rol)
        {
            Rol = rol;
        }
    }
}
