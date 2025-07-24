using Entity.DTOs.Implements.SecurityAuthentication.User;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.User
{
    public class GetAllUserQuery : IRequest<IEnumerable<UserSelectDto>>
    {
    }
}
