using Business.Interfaces.Implements;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.RecuperarContraseña
{
    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Unit>
    {
        private readonly IAuthService _passwordResetService;

        public RequestPasswordResetCommandHandler(IAuthService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        public async Task<Unit> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            await _passwordResetService.RequestPasswordResetAsync(request.Dto.Email);
            return Unit.Value;
        }
    }
}
