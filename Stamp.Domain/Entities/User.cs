using System;

namespace Stamp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer"; // مثلاً "Admin" یا "Customer"
    public bool IsDeleted { get; set; } // برای Soft Delete
    public DateTime CreatedAt { get; set; }

    public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>( );

}