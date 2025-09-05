using MediatR;
using Stamp.Application.DTOs;
using Stamp.Domain.Enums;

namespace Stamp.Application.Commands.Users
{
    /// <summary>
    /// Command request for registering a new user in the system.
    /// </summary>
    public class RegisterUserCommand : IRequest<UserDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Default role is Guest when creating a user
        public RoleEnum Role { get; set; } = RoleEnum.Guest;
    }
}
