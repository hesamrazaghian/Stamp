using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Handlers.Users
{
    /// <summary>
    /// رسیدگی به درخواست ورود کاربر
    /// </summary>
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<string> Handle( LoginUserCommand request, CancellationToken cancellationToken )
        {
            var user = await _userRepository.GetByEmailAndTenantAsync(
                request.Email,
                request.TenantId,
                cancellationToken );

            if( user == null )
                throw new UnauthorizedAccessException( "Invalid credentials" );

            var passwordValid = await _passwordHasher.VerifyPasswordAsync( user.PasswordHash, request.Password );
            if( !passwordValid )
                throw new UnauthorizedAccessException( "Invalid credentials" );

            // 🔹 اطمینان از معتبر بودن نقش کاربر
            var roleEnum = Enum.TryParse<RoleEnum>( user.Role, true, out var parsedRole )
                ? parsedRole
                : RoleEnum.Guest;


            // ✅ اگر TenantId نبود → Guid.Empty
            var tenantId = request.TenantId ?? Guid.Empty;

            return _jwtService.GenerateToken(
                user.Id,
                tenantId,
                roleEnum,
                user.Email
            );
        }
    }
}
