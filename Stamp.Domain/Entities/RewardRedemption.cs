namespace Stamp.Domain.Entities
{
    public class RewardRedemption : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RewardId { get; set; }
        public DateTime RedemptionDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public Reward Reward { get; set; } = null!;
    }
}
