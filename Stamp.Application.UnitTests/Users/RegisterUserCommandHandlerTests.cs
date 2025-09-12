using Stamp.Application.Commands.Users;
using Stamp.Application.Handlers.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;

namespace Stamp.Application.UnitTests.Users
{
    /// <summary>
    /// Unit tests for the RegisterUserCommandHandler.
    /// These tests verify registration logic with mocked dependencies.
    /// </summary>
    public class RegisterUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Register_New_User_When_Data_Is_Valid( )
        {
            // Arrange: create mock objects for dependencies
            var userRepositoryMock = new Mock<IUserRepository>( );
            var passwordHasherMock = new Mock<IPasswordHasher>( );

            // Setup mock: return null when searching by email (means user not exists)
            userRepositoryMock
                .Setup( r => r.GetByEmailAsync( It.IsAny<string>( ), It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( ( User? )null );

            // Setup mock: return a fake hashed password when hashing
            passwordHasherMock
                .Setup( h => h.HashPasswordAsync( It.IsAny<string>( ) ) )
                .ReturnsAsync( "hashed_password" );

            // Create handler instance with mocked dependencies
            var handler = new RegisterUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object
            );

            // Create sample command
            var command = new RegisterUserCommand
            {
                Email = "test@example.com",
                Phone = "09120000000",
                Password = "P@ssword",
                Role = RoleEnum.User // Changed to match your current RoleEnum
            };

            // Act: execute handler with the command
            var result = await handler.Handle( command, CancellationToken.None );

            // Assert: verify result and repository behavior
            result.ShouldNotBeNull( );
            result.Email.ShouldBe( "test@example.com" );
            result.Role.ShouldBe( RoleEnum.User );

            // Ensure repository's AddAsync method was called once with correct data
            userRepositoryMock.Verify( r =>
                r.AddAsync( It.Is<User>( u => u.Email == "test@example.com" ),
                It.IsAny<CancellationToken>( ) ), Times.Once );
        }
    }
}
