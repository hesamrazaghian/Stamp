using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Stamp.Application.Authorization;
using System;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Authorization
{
    /// <summary>
    /// بررسی می‌کند که TenantId کاربر با tenantId موجود در مسیر درخواست یکی باشد.
    /// </summary>
    public class SameTenantHandler : AuthorizationHandler<SameTenantRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SameTenantHandler( IHttpContextAccessor httpContextAccessor )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SameTenantRequirement requirement )
        {
            var tenantIdClaim = context.User.FindFirst( "TenantId" )?.Value;

            var routeTenantId = _httpContextAccessor.HttpContext?
                .GetRouteData( )?.Values[ "tenantId" ]?.ToString( );

            if( !string.IsNullOrEmpty( tenantIdClaim ) &&
                !string.IsNullOrEmpty( routeTenantId ) &&
                string.Equals( tenantIdClaim, routeTenantId, StringComparison.OrdinalIgnoreCase ) )
            {
                context.Succeed( requirement );
            }

            return Task.CompletedTask;
        }
    }
}
