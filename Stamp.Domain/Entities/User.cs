using Stamp.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Stamp.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = RoleEnum.Guest.ToString( );


    public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>( );
}
