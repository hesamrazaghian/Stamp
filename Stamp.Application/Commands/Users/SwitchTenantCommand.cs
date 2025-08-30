using MediatR;

namespace Stamp.Application.Commands.Users;

public class SwitchTenantCommand : IRequest<string>
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
}