using MediatR;

namespace Stamp.Application.Commands.Users
{
    /// <summary>
    /// درخواست ورود کاربر (Multi-Tenant)
    /// </summary>
    public class LoginUserCommand : IRequest<string>
    {
        public string Email { get; set; } = string.Empty;     // ایمیل کاربر
        public string Password { get; set; } = string.Empty;  // رمز عبور
        public Guid? TenantId { get; set; }                    // شناسه سازمان / Tenant
    }
}
