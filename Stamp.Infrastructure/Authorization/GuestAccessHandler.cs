using Microsoft.AspNetCore.Authorization;
using Stamp.Application.Authorization;
using System;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Authorization;

public class GuestAccessHandler : AuthorizationHandler<GuestAccessPolicy>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GuestAccessPolicy requirement )
    {
        // ✅ مهمان یا کاربر عادی هر دو می‌تونن دسترسی داشته باشن
        if( context.User.Identity?.IsAuthenticated == false ||
            context.User.Identity?.IsAuthenticated == true )
        {
            context.Succeed( requirement );
        }

        return Task.CompletedTask;
    }
}