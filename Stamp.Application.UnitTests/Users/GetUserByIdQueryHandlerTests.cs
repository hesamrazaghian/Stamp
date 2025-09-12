using Moq;
using Shouldly;
using Stamp.Application.DTOs;
using Stamp.Application.Handlers.Users;
using Stamp.Application.Interfaces;
using Stamp.Application.Queries.Users;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;
using AutoMapper; // اضافه کردن AutoMapper

namespace Stamp.Application.UnitTests.Users
{
    public class GetUserByIdQueryHandlerTests
    {
        #region Test: Successful fetch
        [Fact]
        public async Task Handle_Should_Return_UserDto_When_User_Found( )
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>( );
            var mapperMock = new Mock<IMapper>( );

            var userId = Guid.NewGuid( );
            var existingUser = new User
            {
                Id = userId,
                Email = "found@example.com",
                Phone = "09120000000",
                Role = RoleEnum.User,
                CreatedAt = DateTime.UtcNow
            };

            userRepositoryMock
                .Setup( r => r.GetByIdAsync( userId, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( existingUser );

            mapperMock
                .Setup( m => m.Map<UserDto>( existingUser ) )
                .Returns( new UserDto
                {
                    Id = existingUser.Id,
                    Email = existingUser.Email,
                    Phone = existingUser.Phone,
                    Role = existingUser.Role,
                    CreatedAt = existingUser.CreatedAt
                } );

            var handler = new GetUserByIdQueryHandler( userRepositoryMock.Object, mapperMock.Object );
            var query = new GetUserByIdQuery { UserId = userId };

            // Act
            var result = await handler.Handle( query, CancellationToken.None );

            // Assert
            result.ShouldNotBeNull( );
            result.ShouldBeOfType<UserDto>( );
            result.Id.ShouldBe( userId );
            result.Email.ShouldBe( "found@example.com" );
            result.Role.ShouldBe( RoleEnum.User );
        }
        #endregion

        #region Test: User not found
        [Fact]
        public async Task Handle_Should_Return_Null_When_User_Not_Found( )
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>( );
            var mapperMock = new Mock<IMapper>( );

            var userId = Guid.NewGuid( );
            userRepositoryMock
                .Setup( r => r.GetByIdAsync( userId, It.IsAny<CancellationToken>( ) ) )
                .ReturnsAsync( ( User? )null );

            var handler = new GetUserByIdQueryHandler( userRepositoryMock.Object, mapperMock.Object );
            var query = new GetUserByIdQuery { UserId = userId };

            // Act
            var result = await handler.Handle( query, CancellationToken.None );

            // Assert
            result.ShouldBeNull( );
        }
        #endregion

        #region Test: Invalid Id
        [Fact]
        public async Task Handle_Should_Throw_ArgumentException_When_Id_Is_Empty( )
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>( );
            var mapperMock = new Mock<IMapper>( );

            var handler = new GetUserByIdQueryHandler( userRepositoryMock.Object, mapperMock.Object );
            var query = new GetUserByIdQuery { UserId = Guid.Empty };

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>( async ( ) =>
            {
                await handler.Handle( query, CancellationToken.None );
            } );
        }
        #endregion
    }
}
