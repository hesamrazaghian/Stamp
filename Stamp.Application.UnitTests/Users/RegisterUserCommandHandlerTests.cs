using Moq;
using Shouldly;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
using Stamp.Application.Exceptions;
using Stamp.Application.Handlers.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;

namespace Stamp.Application.UnitTests.Users
{
    public class RegisterUserCommandHandlerTests
    {
        #region Test: Successful Registration
        [Fact]
        public async Task Handle_Should_Create_New_User_When_Data_Is_Valid( )
        {
            // ========== Arrange ==========
            var userRepositoryMock = new Mock<IUserRepository>( );
            var passwordHasherMock = new Mock<IPasswordHasher>( );

            var command = new RegisterUserCommand
            {
                Email = "newuser@example.com",
                Phone = "09120000000",
                Password = "ValidP@ss123",
                Role = RoleEnum.User
            };

            // Mock: Ensure no existing user with the same email
            userRepositoryMock
                .Setup( r => r.GetByEmailAsync( command.Email, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( ( User? )null );

            // Mock: Hash the password
            passwordHasherMock
                .Setup( h => h.HashPasswordAsync( command.Password ) )
                .ReturnsAsync( "hashed_password" );

            // Mock: Add user should succeed
            userRepositoryMock
                .Setup( r => r.AddAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ) )
                .Returns( Task.CompletedTask );

            var handler = new RegisterUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object
            );

            // ========== Act ==========
            var result = await handler.Handle( command, CancellationToken.None );

            // ========== Assert ==========
            result.ShouldNotBeNull( );
            result.ShouldBeOfType<UserDto>( );
            result.Email.ShouldBe( command.Email );
            result.Phone.ShouldBe( command.Phone );
            result.Role.ShouldBe( RoleEnum.User );

            // Verify persistence call
            userRepositoryMock.Verify( r => r.AddAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ), Times.Once );
        }
        #endregion

        #region Test: Duplicate Email
        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Email_Already_Exists( )
        {
            // ========== Arrange ==========
            var userRepositoryMock = new Mock<IUserRepository>( );
            var passwordHasherMock = new Mock<IPasswordHasher>( );

            var command = new RegisterUserCommand
            {
                Email = "existing@example.com",
                Phone = "09120000000",
                Password = "SomePass1!",
                Role = RoleEnum.User
            };

            // Mock: Simulate existing user in repository
            userRepositoryMock
                .Setup( r => r.GetByEmailAsync( command.Email, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( new User
                {
                    Id = Guid.NewGuid( ),
                    Email = command.Email,
                    PasswordHash = "oldhash",
                    Role = RoleEnum.User
                } );

            var handler = new RegisterUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object
            );

            // ========== Act & Assert ==========
            var ex = await Should.ThrowAsync<Exception>( async ( ) =>
            {
                await handler.Handle( command, CancellationToken.None );
            } );

            ex.Message.ShouldBe( "A user with this email already exists." );

            // Ensure new user is not added
            userRepositoryMock.Verify( r => r.AddAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ), Times.Never );
        }
        #endregion

        #region Test: Invalid Role Assignment
        [Fact]
        public async Task Handle_Should_Throw_InvalidRoleAssignmentException_When_Assigning_Admin_Role( )
        {
            // ========== Arrange ==========
            var userRepositoryMock = new Mock<IUserRepository>( );
            var passwordHasherMock = new Mock<IPasswordHasher>( );

            var command = new RegisterUserCommand
            {
                Email = "adminuser@example.com",
                Phone = "09120000000",
                Password = "AdminP@ss1!",
                Role = RoleEnum.Admin // Not allowed
            };

            // Mock: Ensure no existing user
            userRepositoryMock
                .Setup( r => r.GetByEmailAsync( command.Email, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( ( User? )null );

            var handler = new RegisterUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object
            );

            // ========== Act & Assert ==========
            var ex = await Should.ThrowAsync<InvalidRoleAssignmentException>( async ( ) =>
            {
                await handler.Handle( command, CancellationToken.None );
            } );

            ex.Message.ShouldBe( "You are not allowed to assign Admin role." );

            // Verify AddAsync never called
            userRepositoryMock.Verify( r => r.AddAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ), Times.Never );
        }
        #endregion
    }
}
