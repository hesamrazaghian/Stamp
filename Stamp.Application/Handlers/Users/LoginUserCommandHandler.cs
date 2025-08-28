using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;

namespace Stamp.Application.Handlers.Users
{
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
            // اگر Multi-Tenant هستیم:
            var user = await _userRepository.GetByEmailAndTenantAsync(
                request.Email,
                request.TenantId,   // این باید توی LoginUserCommand موجود باشه
                cancellationToken );

            if( user == null )
                throw new UnauthorizedAccessException( "Invalid credentials" );

            var passwordValid = await _passwordHasher.VerifyPasswordAsync(
                user.PasswordHash,
                request.Password );

            if( !passwordValid )
                throw new UnauthorizedAccessException( "Invalid credentials" );

            // گرفتن TenantId از ورودی یا مرتبط با کاربر
            var tenantId = request.TenantId;

            return _jwtService.GenerateToken(
                user.Id,
                tenantId,
                user.Role,
                user.Email // اضافه شد
            );
        }
    }
}
