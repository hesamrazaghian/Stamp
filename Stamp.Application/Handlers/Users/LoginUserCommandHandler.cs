using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;

namespace Stamp.Application.Handlers.Users
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
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

        public async Task<LoginResultDto> Handle( LoginUserCommand request, CancellationToken cancellationToken )
        {
            // Step 1: Check if user exists by email
            var user = await _userRepository.GetByEmailAsync( request.Email, cancellationToken );

            if( user == null )
                throw new UnauthorizedAccessException( "Invalid email or password" );

            // Step 2: Validate password
            var passwordValid = await _passwordHasher.VerifyPasswordAsync( user.PasswordHash, request.Password );
            if( !passwordValid )
                throw new UnauthorizedAccessException( "Invalid email or password" );

            // Step 3: Generate JWT token
            var token = _jwtService.GenerateToken( user.Id, user.Role, user.Email );

            // Step 4: Return login result
            return new LoginResultDto
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role.ToString( ),
                Token = token
            };
        }
    }
}
