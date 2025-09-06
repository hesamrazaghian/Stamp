using Stamp.Domain.Enums;

namespace Stamp.Application.Interfaces
{
    public interface IJwtService
    {
        // Generate JWT token with default expiration time from appsettings.json
        string GenerateToken( Guid userId, RoleEnum role, string? email );

        // Generate JWT token with custom expiration time in minutes
        string GenerateToken( Guid userId, RoleEnum role, string? email, int expireMinutes );
    }
}
