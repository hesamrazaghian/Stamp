using Microsoft.AspNetCore.Authorization;

namespace Stamp.Application.Authorization
{
    /// <summary>
    /// شرط اینکه کاربر فقط به منابع Tenant خودش دسترسی داشته باشد.
    /// </summary>
    public class SameTenantRequirement : IAuthorizationRequirement
    {
    }
}
