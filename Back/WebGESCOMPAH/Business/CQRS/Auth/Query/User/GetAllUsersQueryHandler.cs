using Business.Interfaces.Implements;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MediatR;
using WebGESCOMPAH.Extensions.User;

namespace Business.CQRS.Auth.Query.User
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUserQuery, IEnumerable<UserSelectDto>>
    {
        private readonly IUserService _userService;

        public GetAllUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IEnumerable<UserSelectDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetAllUser();
        }
    }
}
