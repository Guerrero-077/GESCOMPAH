using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using MediatR;
namespace Business.CQRS.SecrutityAuthentication.RecuperarContraseña
{
    public class ResetPasswordCommand : IRequest<Unit>
    {
        public ConfirmResetDto Dto { get; }

        public ResetPasswordCommand(ConfirmResetDto dto)
        {
            Dto = dto;
        }
    }

}
