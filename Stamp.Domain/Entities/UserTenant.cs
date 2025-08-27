using System;

namespace Stamp.Domain.Entities
{
    public class UserTenant
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public string Role { get; set; } = "Customer";
        public int Points { get; set; } = 0;

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
