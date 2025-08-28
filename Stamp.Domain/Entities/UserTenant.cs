using Stamp.Domain.Entities;

public class UserTenant : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;

    public string Role { get; set; } = "Customer";
    public int TotalStamps { get; set; } = 0;
}
