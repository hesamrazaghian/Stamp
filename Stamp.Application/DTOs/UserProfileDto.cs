using Stamp.Domain.Enums;

namespace Stamp.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public RoleEnum Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TenantMembershipDto> Tenants { get; set; } = new( );
}
