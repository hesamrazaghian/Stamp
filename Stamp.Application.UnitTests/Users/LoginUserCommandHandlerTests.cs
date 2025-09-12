using Moq;
using Shouldly;
using Stamp.Application.Commands.Users;
using Stamp.Application.Handlers.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;

namespace Stamp.Application.UnitTests.Users
{
    public class LoginUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_Token_When_Credentials_Are_Correct( )
        {
            // Arrange - create mocks
            var userRepositoryMock = new Mock<IUserRepository>( );
            var passwordHasherMock = new Mock<IPasswordHasher>( );
            var jwtServiceMock = new Mock<IJwtService>( );

            // Arrange - prepare a fake existing user
            var existingUser = new User
            {
                Id = Guid.NewGuid( ),
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                Role = RoleEnum.User
            };

            // Arrange - mock GetByEmailAsync to return the existing user
            userRepositoryMock
                .Setup( r => r.GetByEmailAsync( "test@example.com", It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( existingUser );

            // Arrange - mock password verification to return true
            passwordHasherMock
                .Setup( h => h.VerifyPasswordAsync( existingUser.PasswordHash, "P@ssword" ) )
                .ReturnsAsync( true );

            // Arrange - mock JWT generation to return a fake token
            jwtServiceMock
                .Setup( j => j.GenerateToken( existingUser.Id, existingUser.Role, existingUser.Email ) )
                .Returns( "fake_jwt_token" );

            // Arrange - create handler with mocks
            var handler = new LoginUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                jwtServiceMock.Object
            );

            // Act - execute the handler
            var command = new LoginUserCommand
            {
                Email = "test@example.com",
                Password = "P@ssword"
            };
            var result = await handler.Handle( command, CancellationToken.None );

            // Assert - verify expected output
            result.ShouldNotBeNull( );
            result.Token.ShouldBe( "fake_jwt_token" );
            result.Email.ShouldBe( "test@example.com" );
            result.Role.ShouldBe( "User" );
        }

        [Fact]
        public async Task Handle_Should_Throw_UnauthorizedAccessException_When_Email_Is_Invalid( )
        {
            // Arrange - create mocks
            var userRepositoryMock = new Mock<IUserRepository>( );
            var passwordHasherMock = new Mock<IPasswordHasher>( );
            var jwtServiceMock = new Mock<IJwtService>( );

            // Arrange - mock GetByEmailAsync to return null (user not found)
            userRepositoryMock
                .Setup( r => r.GetByEmailAsync( "notfound@example.com", It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( ( User? )null );

            // Arrange - create handler with mocks
            var handler = new LoginUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                jwtServiceMock.Object
            );

            // Prepare the command with non-existing email
            var command = new LoginUserCommand
            {
                Email = "notfound@example.com",
                Password = "P@ssword"
            };

            // Act & Assert - expect UnauthorizedAccessException
            var ex = await Should.ThrowAsync<UnauthorizedAccessException>( async ( ) =>
            {
                await handler.Handle( command, CancellationToken.None );
            } );

            // Assert - verify the exception message
            ex.Message.ShouldBe( "Invalid email or password" );

            // Verify that password verification and token generation are never called
            passwordHasherMock.Verify(
                p => p.VerifyPasswordAsync( It.IsAny<string>( ), It.IsAny<string>( ) ),
                Times.Never
            );
            jwtServiceMock.Verify(
                j => j.GenerateToken( It.IsAny<Guid>( ), It.IsAny<RoleEnum>( ), It.IsAny<string?>( ) ),
                Times.Never
            );
        }
    }
}
