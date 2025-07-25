﻿using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.User
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
