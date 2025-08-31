using Microsoft.AspNetCore.Authorization;
using Stamp.Application.Authorization;
using Stamp.Domain.Enums;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Authorization
{
    public class GuestAccessHandler : AuthorizationHandler<GuestAccessPolicy>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GuestAccessPolicy requirement )
        {
            var role = context.User.FindFirst( ClaimTypes.Role )?.Value;

            // اگر نقش موجود و قابل Parse بود
            if( !string.IsNullOrWhiteSpace( role ) &&
                Enum.TryParse<RoleEnum>( role, true, out var roleEnum ) )
            {
                // اگر نقش Guest بود یا کاربر وارد سیستم شده بود
                if( roleEnum == RoleEnum.Guest || context.User.Identity?.IsAuthenticated == true )
                {
                    context.Succeed( requirement );
                }
            }

            return Task.CompletedTask;
        }
    }
}
