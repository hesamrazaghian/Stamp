using System;

namespace Stamp.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken( Guid userId, Guid tenantId, string role, string email );

}