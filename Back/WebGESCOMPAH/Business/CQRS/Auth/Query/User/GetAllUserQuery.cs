using Entity.DTOs.Implements.SecurityAuthentication.User;
using MediatR;

namespace WebGESCOMPAH.Extensions.User
{
    public class GetAllUserQuery : IRequest<IEnumerable<UserSelectDto>>
    {
    }
}
