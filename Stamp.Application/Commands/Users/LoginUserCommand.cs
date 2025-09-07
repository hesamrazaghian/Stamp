using MediatR;
using Stamp.Application.DTOs;

namespace Stamp.Application.Commands.Users
{
    // Command model for user login request
    public class LoginUserCommand : IRequest<LoginResultDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
