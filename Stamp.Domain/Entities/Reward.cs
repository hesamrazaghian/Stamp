namespace Stamp.Domain.Entities
{
    public class Reward : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int RequiredStamps { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ICollection<RewardRedemption> RewardRedemptions { get; set; } = new List<RewardRedemption>( );
    }
}
