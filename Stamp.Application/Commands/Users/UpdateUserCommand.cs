using MediatR;
using Stamp.Application.DTOs;
using Stamp.Domain.Enums;

namespace Stamp.Application.Commands.Users
{
    /// <summary>
    /// Command for updating user details.
    /// </summary>
    public class UpdateUserCommand : IRequest<UserDto?>
    {
        // The Id of the user to update
        public Guid UserId { get; set; }

        // Updated email (optional — if empty, should keep old one)
        public string Email { get; set; } = string.Empty;

        // Updated phone number (optional)
        public string Phone { get; set; } = string.Empty;

        // Optional role update
        public RoleEnum? Role { get; set; }
    }
}
