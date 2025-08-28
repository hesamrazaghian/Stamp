using System;
using System.Collections.Generic;

namespace Stamp.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer"; // مثلاً "Admin" یا "Customer"

    public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>( );
}
