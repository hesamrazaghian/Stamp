using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Stamp.Application.Authorization;

namespace Stamp.Infrastructure.Authorization;

public class SameTenantHandler : AuthorizationHandler<SameTenantRequirement, Guid>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SameTenantHandler( IHttpContextAccessor httpContextAccessor )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameTenantRequirement requirement,
        Guid resourceTenantId )
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if( httpContext == null )
        {
            return Task.CompletedTask;
        }

        var tenantClaim = context.User.FindFirst( "TenantId" )?.Value;
        if( tenantClaim != null && Guid.TryParse( tenantClaim, out var userTenantId ) )
        {
            if( userTenantId == resourceTenantId )
            {
                context.Succeed( requirement );
            }
        }
        return Task.CompletedTask;
    }
}