using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Stamp.Application.Authorization;
using Stamp.Domain.Enums; // ✅ استفاده از RoleEnum

namespace Stamp.Infrastructure.Authorization
{
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
            // گرفتن نقش کاربر به صورت امن
            var roleClaim = context.User.FindFirst( ClaimTypes.Role )?.Value;

            if( !string.IsNullOrWhiteSpace( roleClaim ) &&
                Enum.TryParse<RoleEnum>( roleClaim, out var roleEnum ) )
            {
                // اگر نقش Guest بود، اینجا دسترسی مهمان رو میدیم
                if( roleEnum == RoleEnum.Guest )
                {
                    context.Succeed( requirement );
                    return Task.CompletedTask;
                }
            }

            // بررسی TenantId
            var tenantClaim = context.User.FindFirst( "TenantId" )?.Value;
            if( !string.IsNullOrWhiteSpace( tenantClaim ) &&
                Guid.TryParse( tenantClaim, out var userTenantId ) )
            {
                if( userTenantId == resourceTenantId )
                {
                    context.Succeed( requirement );
                }
            }

            return Task.CompletedTask;
        }
    }
}
