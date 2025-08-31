using Stamp.Domain.Enums;
using System.Collections.Generic;

namespace Stamp.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public RoleEnum Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<UserTenantDto> Tenants { get; set; } = new List<UserTenantDto>( );
}