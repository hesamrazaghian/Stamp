using Stamp.Domain.Entities;

public class StampTransaction : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Type { get; set; } = string.Empty; // مثلا "Earn" یا "Redeem"
    public int Quantity { get; set; }               // تعداد مهر
    public string? Description { get; set; }

    public Tenant Tenant { get; set; } = null!;
}
