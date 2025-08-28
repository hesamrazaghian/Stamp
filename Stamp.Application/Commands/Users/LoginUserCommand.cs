using MediatR;
using Stamp.Application.DTOs;

namespace Stamp.Application.Commands.Users
{
    public class LoginUserCommand : IRequest<string> // برمی‌گردونه JWT Token
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }
}
