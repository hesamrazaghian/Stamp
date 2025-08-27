using System;
using System.Collections.Generic;

namespace Stamp.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty; // مثلا "آرایشگاه" یا "کارواش"
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>( );
    }
}
