using Moq;
using Shouldly;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
using Stamp.Application.Handlers.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;
using AutoMapper;

namespace Stamp.Application.UnitTests.Users
{
    public class UpdateUserCommandHandlerTests
    {
        #region Test: Successful Update of Existing User
        [Fact]
        public async Task Handle_Should_Update_User_When_User_Exists( )
        {
            // ========== Arrange ==========
            var userRepositoryMock = new Mock<IUserRepository>( );
            var mapperMock = new Mock<IMapper>( );

            var existingUser = new User
            {
                Id = Guid.NewGuid( ),
                Email = "old@example.com",
                Phone = "09120000000",
                Role = RoleEnum.User
            };

            userRepositoryMock
                .Setup( r => r.GetByIdAsync( existingUser.Id, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( existingUser );

            userRepositoryMock
                .Setup( r => r.UpdateAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ) )
                .Returns( Task.CompletedTask );

            mapperMock
                .Setup( m => m.Map<UserDto>( It.IsAny<User>( ) ) )
                .Returns( ( User u ) => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role
                } );

            var command = new UpdateUserCommand
            {
                UserId = existingUser.Id,
                Email = "new@example.com",
                Phone = "09998887777",
                Role = RoleEnum.Admin
            };

            var handler = new UpdateUserCommandHandler( userRepositoryMock.Object, mapperMock.Object );

            // ========== Act ==========
            var result = await handler.Handle( command, CancellationToken.None );

            // ========== Assert ==========
            result.ShouldNotBeNull( );
            result.Id.ShouldBe( existingUser.Id );
            result.Email.ShouldBe( "new@example.com" );
            result.Phone.ShouldBe( "09998887777" );
            result.Role.ShouldBe( RoleEnum.Admin );

            userRepositoryMock.Verify( r => r.UpdateAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ), Times.Once );
        }
        #endregion

        #region Test: User Not Found
        [Fact]
        public async Task Handle_Should_Return_Null_When_User_Not_Found( )
        {
            // ========== Arrange ==========
            var userRepositoryMock = new Mock<IUserRepository>( );
            var mapperMock = new Mock<IMapper>( );

            userRepositoryMock
                .Setup( r => r.GetByIdAsync( It.IsAny<Guid>( ), It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( ( User? )null );

            var command = new UpdateUserCommand
            {
                UserId = Guid.NewGuid( ),
                Email = "doesnotexist@example.com",
                Phone = "09000000000",
                Role = RoleEnum.User
            };

            var handler = new UpdateUserCommandHandler( userRepositoryMock.Object, mapperMock.Object );

            // ========== Act ==========
            var result = await handler.Handle( command, CancellationToken.None );

            // ========== Assert ==========
            result.ShouldBeNull( );
            userRepositoryMock.Verify( r => r.UpdateAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ), Times.Never );
        }
        #endregion

        #region Test: Partial Update (Only Email Provided)
        [Fact]
        public async Task Handle_Should_Update_Only_Email_When_Only_Email_Provided( )
        {
            // ========== Arrange ==========
            var userRepositoryMock = new Mock<IUserRepository>( );
            var mapperMock = new Mock<IMapper>( );

            var existingUser = new User
            {
                Id = Guid.NewGuid( ),
                Email = "old@example.com",
                Phone = "09120000000",
                Role = RoleEnum.User
            };

            userRepositoryMock
                .Setup( r => r.GetByIdAsync( existingUser.Id, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( existingUser );

            userRepositoryMock
                .Setup( r => r.UpdateAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ) )
                .Returns( Task.CompletedTask );

            mapperMock
                .Setup( m => m.Map<UserDto>( It.IsAny<User>( ) ) )
                .Returns( ( User u ) => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role
                } );

            var command = new UpdateUserCommand
            {
                UserId = existingUser.Id,
                Email = "updated@example.com"
                // Phone and Role are not provided, should remain unchanged
            };

            var handler = new UpdateUserCommandHandler( userRepositoryMock.Object, mapperMock.Object );

            // ========== Act ==========
            var result = await handler.Handle( command, CancellationToken.None );

            // ========== Assert ==========
            result.ShouldNotBeNull( );
            result.Id.ShouldBe( existingUser.Id );
            result.Email.ShouldBe( "updated@example.com" );
            result.Phone.ShouldBe( "09120000000" ); // unchanged
            result.Role.ShouldBe( RoleEnum.User );  // unchanged

            userRepositoryMock.Verify( r => r.UpdateAsync( It.IsAny<User>( ), It.IsAny<CancellationToken>( ) ), Times.Once );
        }
        #endregion
    }
}
