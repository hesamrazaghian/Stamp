using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;
using Stamp.Application.Commands.Users;
using Stamp.Application.Handlers.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Enums;
using Stamp.Domain.Entities;

namespace Stamp.Application.UnitTests.Users
{
    public class ChangeUserRoleCommandHandlerTests
    {
        #region Fields

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ChangeUserRoleCommandHandler _handler;

        #endregion

        #region Constructor

        public ChangeUserRoleCommandHandlerTests( )
        {
            _userRepositoryMock = new Mock<IUserRepository>( );
            _handler = new ChangeUserRoleCommandHandler( _userRepositoryMock.Object );
        }

        #endregion

        #region Tests

        [Fact]
        public async Task Handle_Should_Return_True_When_User_Role_Is_Changed_Successfully( )
        {
            // Arrange
            var userId = Guid.NewGuid( );
            var existingUser = new User { Id = userId, Role = RoleEnum.User };
            var command = new ChangeUserRoleCommand
            {
                UserId = userId,
                NewRole = RoleEnum.Admin
            };

            _userRepositoryMock
                .Setup( repo => repo.GetByIdAsync( userId, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( existingUser );

            _userRepositoryMock
                .Setup( repo => repo.UpdateUserRoleAsync( userId, RoleEnum.Admin, It.IsAny<CancellationToken>( ) ) )
                .Returns( Task.CompletedTask );

            // Act
            var result = await _handler.Handle( command, CancellationToken.None );

            // Assert
            result.ShouldBeTrue( );
            _userRepositoryMock.Verify( r => r.UpdateUserRoleAsync( userId, RoleEnum.Admin, It.IsAny<CancellationToken>( ) ), Times.Once );
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_User_Not_Found( )
        {
            // Arrange
            var userId = Guid.NewGuid( );
            var command = new ChangeUserRoleCommand
            {
                UserId = userId,
                NewRole = RoleEnum.Admin
            };

            _userRepositoryMock
                .Setup( repo => repo.GetByIdAsync( userId, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( ( User )null );

            // Act
            var result = await _handler.Handle( command, CancellationToken.None );

            // Assert
            result.ShouldBeFalse( );
            _userRepositoryMock.Verify( r => r.UpdateUserRoleAsync( It.IsAny<Guid>( ), It.IsAny<RoleEnum>( ), It.IsAny<CancellationToken>( ) ), Times.Never );
        }

        [Fact]
        public async Task Handle_Should_Return_True_When_Role_Is_Same_As_Current( )
        {
            // Arrange
            var userId = Guid.NewGuid( );
            var existingUser = new User { Id = userId, Role = RoleEnum.User };
            var command = new ChangeUserRoleCommand
            {
                UserId = userId,
                NewRole = RoleEnum.User
            };

            _userRepositoryMock
                .Setup( repo => repo.GetByIdAsync( userId, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( existingUser );

            _userRepositoryMock
                .Setup( repo => repo.UpdateUserRoleAsync( userId, RoleEnum.User, It.IsAny<CancellationToken>( ) ) )
                .Returns( Task.CompletedTask );

            // Act
            var result = await _handler.Handle( command, CancellationToken.None );

            // Assert
            result.ShouldBeTrue( );
            _userRepositoryMock.Verify( r => r.UpdateUserRoleAsync( userId, RoleEnum.User, It.IsAny<CancellationToken>( ) ), Times.Once );
        }

        #endregion
    }
}
