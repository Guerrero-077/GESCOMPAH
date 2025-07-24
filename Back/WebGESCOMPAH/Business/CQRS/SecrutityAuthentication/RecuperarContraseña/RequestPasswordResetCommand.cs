using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.RecuperarContraseña
{
    public class RequestPasswordResetCommand : IRequest<Unit>
    {
        public RequestResetDto Dto { get; }

        public RequestPasswordResetCommand(RequestResetDto dto)
        {
            Dto = dto;
        }
    }
}
