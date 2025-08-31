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
            // گرفتن نقش کاربر
            var roleClaim = context.User.FindFirst( ClaimTypes.Role )?.Value;

            // اگر نقش معتبر و قابل Parse با RoleEnum بود (case-insensitive)
            if( !string.IsNullOrWhiteSpace( roleClaim ) &&
                Enum.TryParse<RoleEnum>( roleClaim, true, out var roleEnum ) )
            {
                // نقش Guest → اجازه دسترسی می‌دهیم
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
