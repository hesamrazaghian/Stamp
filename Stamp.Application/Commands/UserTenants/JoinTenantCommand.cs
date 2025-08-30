using MediatR;

namespace Stamp.Application.Commands.UserTenants;

public class JoinTenantCommand : IRequest
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
}