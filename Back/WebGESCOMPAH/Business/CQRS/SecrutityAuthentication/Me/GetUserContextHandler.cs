using Business.CQRS.SecrutityAuthentication.Me;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication;
using MediatR;

public class GetUserContextHandler : IRequestHandler<GetUserContextQuery, UserMeDto>
{
    private readonly IAuthService _authService;

    public GetUserContextHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<UserMeDto> Handle(GetUserContextQuery request, CancellationToken cancellationToken)
    {
        return await _authService.BuildUserContextAsync(request.UserId);
    }
}
