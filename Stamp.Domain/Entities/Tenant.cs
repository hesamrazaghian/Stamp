using System;
using System.Collections.Generic;

namespace Stamp.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty; // مثلا "آرایشگاه" یا "کارواش"

    public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>( );
}
