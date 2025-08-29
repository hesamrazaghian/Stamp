using MediatR;
using Stamp.Application.DTOs;

namespace Stamp.Application.Commands.Users;

public class RegisterUserCommand : IRequest<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    public Guid? TenantId { get; set; }
}
