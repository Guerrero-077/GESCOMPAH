using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using MediatR;

namespace Business.CQRS.Auth.Commands.Login
{
    public class LoginCommand : LoginDto, IRequest<string>
    {
        // Extiende Email y Password
    }
}
