using Business.Interfaces.Implements;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.RecuperarContraseña
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IAuthService _passwordResetService;

        public ResetPasswordCommandHandler(IAuthService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            await _passwordResetService.ResetPasswordAsync(request.Dto);
            return Unit.Value;
        }
    }
}
