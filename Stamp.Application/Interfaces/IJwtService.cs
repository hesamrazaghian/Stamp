using Stamp.Domain.Enums;
using System;

namespace Stamp.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken( Guid userId, Guid tenantId, RoleEnum role, string email );
}
