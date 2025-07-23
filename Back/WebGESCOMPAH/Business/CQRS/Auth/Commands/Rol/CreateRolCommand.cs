using Entity.DTOs.Implements.Persons.Peron;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CQRS.Auth.Commands.Rol
{
    public class CreateRolCommand : IRequest<int> // Devuelve el ID de la nueva persona
    {
        public RolDto Rol { get; set; }

        public CreateRolCommand(RolDto rol)
        {
            Rol = rol;
        }
    }
}
