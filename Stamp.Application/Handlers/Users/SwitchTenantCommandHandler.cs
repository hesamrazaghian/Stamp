using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Handlers.Users
{
    public class SwitchTenantCommandHandler : IRequestHandler<SwitchTenantCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IJwtService _jwtService;

        public SwitchTenantCommandHandler(
            IUserRepository userRepository,
            ITenantRepository tenantRepository,
            IJwtService jwtService )
        {
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _jwtService = jwtService;
        }

        public async Task<string> Handle(
            SwitchTenantCommand command,
            CancellationToken cancellationToken )
        {
            // ✅ بررسی وجود Tenant
            var tenant = await _tenantRepository.GetByIdAsync( command.TenantId, cancellationToken );
            if( tenant == null )
                throw new Exception( "Tenant not found" );

            // ✅ بررسی عضویت کاربر در Tenant
            var isMember = await _userRepository.ExistsInTenantAsync(
                command.UserId,
                command.TenantId,
                cancellationToken );

            if( !isMember )
                throw new Exception( "User is not a member of this tenant" );

            // ✅ دریافت اطلاعات کاربر
            var user = await _userRepository.GetByIdAsync( command.UserId, cancellationToken );
            if( user == null )
                throw new Exception( "User not found" );

            // 🔹 تبدیل رشته‌ی Role به Enum
            if( !Enum.TryParse<RoleEnum>( user.Role, out var roleEnum ) )
            {
                roleEnum = RoleEnum.Guest; // پیش‌فرض اگر مقدار معتبر نبود
            }

            // ✅ ایجاد کلیم‌ها با TenantId جدید (در صورت نیاز)
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("Role", roleEnum.ToString()),
                new Claim("TenantId", command.TenantId.ToString())
            };

            // ✅ تولید توکن جدید
            return _jwtService.GenerateToken(
                user.Id,
                command.TenantId,
                roleEnum, 
                user.Email
            );
        }
    }
}
