namespace Business.CQRS.Auth.Query.Me
{
    using Entity.DTOs.Implements.SecurityAuthentication;
    using MediatR;

    public record GetUserContextQuery(int UserId) : IRequest<UserMeDto>;

}
