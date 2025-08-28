namespace Stamp.Application.DTOs;

public class TenantMembershipDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
}
