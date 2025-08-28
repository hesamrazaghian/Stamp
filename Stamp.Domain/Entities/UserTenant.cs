using System;

namespace Stamp.Domain.Entities;

public class UserTenant : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // TenantId از BaseEntity میاد

    public Tenant Tenant { get; set; } = null!;

    public string Role { get; set; } = "Customer";
    public int Points { get; set; } = 0;
}
