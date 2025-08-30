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
        // ✅ خط ۱: دریافت نقش کاربر از JWT
        var roleClaim = context.User.FindFirst( "Role" )?.Value;

        // ✅ خط ۲: بررسی اینکه آیا کاربر Guest است
        if( roleClaim == "Guest" )
        {
            context.Succeed( requirement );
            return Task.CompletedTask;
        }

        // ✅ خط ۳: بررسی TenantId برای کاربران عادی
        var tenantClaim = context.User.FindFirst( "TenantId" )?.Value;
        if( tenantClaim != null && Guid.TryParse( tenantClaim, out var userTenantId ) )
        {
            // ✅ خط ۴: تأیید تطابق TenantId
            if( userTenantId == resourceTenantId )
            {
                context.Succeed( requirement );
            }
        }

        // ✅ خط ۵: خروج ایمن از متد
        return Task.CompletedTask;
    }
}