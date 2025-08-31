using Stamp.Domain.Entities;
using Stamp.Domain.Enums;

public class UserTenant : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public string Role { get; set; } = RoleEnum.Guest.ToString( ); // پیش‌فرض Guest
    public int TotalStamps { get; set; } = 0;
}
