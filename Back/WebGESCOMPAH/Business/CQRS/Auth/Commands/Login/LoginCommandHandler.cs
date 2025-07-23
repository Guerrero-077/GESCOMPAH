using Business.Interfaces;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Mapster;
using MediatR;

namespace Business.CQRS.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IToken _tokenService;

        public LoginCommandHandler(IToken tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Usamos Mapster para mapear LoginCommand a LoginDto
            var loginDto = request.Adapt<LoginDto>();

            var token = await _tokenService.GenerateToken(loginDto);

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Credenciales inválidas");

            return token;
        }
    }
}
