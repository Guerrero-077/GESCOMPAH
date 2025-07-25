﻿using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Entity.Enum;
using MediatR;

namespace Business.CQRS.SecrutityAuthentication.Register
{
    public class RegisterCommand : RegisterDto, IRequest<UserDto> { }
}
