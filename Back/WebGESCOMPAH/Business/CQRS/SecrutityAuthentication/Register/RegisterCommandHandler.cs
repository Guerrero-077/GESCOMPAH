using Business.Interfaces.Implements;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.Register
{

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(request);
        }
    }
}
