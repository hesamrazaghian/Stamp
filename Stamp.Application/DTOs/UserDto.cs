using Stamp.Domain.Enums;
using System;

namespace Stamp.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user details returned to the client.
    /// </summary>
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public RoleEnum Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
