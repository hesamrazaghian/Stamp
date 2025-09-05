using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;

namespace Stamp.Application.Handlers.Users
{
    /// <summary>
    /// Handles the business logic for registering a new user.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle( RegisterUserCommand request, CancellationToken cancellationToken )
        {
            // 1. Check if user already exists by email
            var existingUser = await _userRepository.GetByEmailAsync( request.Email, cancellationToken );
            if( existingUser != null )
            {
                throw new Exception( "A user with this email already exists." );
            }

            // 2. Hash password securely using IPasswordHasher service
            var passwordHash = await _passwordHasher.HashPasswordAsync( request.Password );

            // 3. Create the User entity instance
            var newUser = new User
            {
                Id = Guid.NewGuid( ),
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                Role = request.Role,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            // 4. Save user in database through IUserRepository
            await _userRepository.AddAsync( newUser, cancellationToken );

            // 5. Return mapped DTO (response object to send back)
            return new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Phone = newUser.Phone,
                Role = newUser.Role,
                CreatedAt = newUser.CreatedAt
            };
        }
    }
}
