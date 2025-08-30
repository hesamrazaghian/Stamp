using Microsoft.AspNetCore.Http;
using System;
using Stamp.Application.Interfaces;

namespace Stamp.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService( IHttpContextAccessor httpContextAccessor )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetCurrentTenantId( )
    {
        var tenantIdClaim = _httpContextAccessor.HttpContext?
            .User.FindFirst( "TenantId" )?.Value;

        return Guid.TryParse( tenantIdClaim, out var tenantId ) ? tenantId : ( Guid? )null;
    }
}