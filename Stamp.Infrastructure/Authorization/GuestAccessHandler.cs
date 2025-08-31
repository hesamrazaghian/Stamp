using Microsoft.AspNetCore.Authorization;
using Stamp.Application.Authorization;
using Stamp.Domain.Enums;
using System;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Authorization;

public class GuestAccessHandler : AuthorizationHandler<GuestAccessPolicy>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GuestAccessPolicy requirement )
    {
        var role = context.User.FindFirst( ClaimTypes.Role )?.Value;

        if( !string.IsNullOrWhiteSpace( role ) &&
                ( role == RoleEnum.Guest.ToString( ) || context.User.Identity?.IsAuthenticated == true ))
        {
            context.Succeed( requirement );
        }

        return Task.CompletedTask;
    }
}