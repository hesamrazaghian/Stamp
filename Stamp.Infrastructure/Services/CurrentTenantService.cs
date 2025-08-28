using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Stamp.Application.Interfaces;

namespace Stamp.Infrastructure.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public Guid TenantId { get; }

        public CurrentTenantService( IHttpContextAccessor httpContextAccessor )
        {
            var tenantClaim = httpContextAccessor.HttpContext?.User?
                .FindFirst( "TenantId" )?.Value;

            if( !string.IsNullOrEmpty( tenantClaim ) && Guid.TryParse( tenantClaim, out var tenantId ) )
                TenantId = tenantId;
            else
                TenantId = Guid.Empty; // یا می‌تونی Exception بندازی
        }
    }
}
