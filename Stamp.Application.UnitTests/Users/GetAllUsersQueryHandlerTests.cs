using AutoMapper;
using Moq;
using Shouldly;
using Stamp.Application.DTOs;
using Stamp.Application.Handlers.Users;
using Stamp.Application.Interfaces;
using Stamp.Application.Queries.Users;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;
using Xunit;

namespace Stamp.Application.UnitTests.Users
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests( )
        {
            _userRepositoryMock = new Mock<IUserRepository>( );
            _mapperMock = new Mock<IMapper>( );

            _handler = new GetAllUsersQueryHandler(
                _userRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_Return_Paged_UserDto_List_When_Users_Exist( )
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "a@test.com", Phone = "111", Role = RoleEnum.User },
                new User { Id = Guid.NewGuid(), Email = "b@test.com", Phone = "222", Role = RoleEnum.Admin },
                new User { Id = Guid.NewGuid(), Email = "c@test.com", Phone = "333", Role = RoleEnum.User }
            };

            _userRepositoryMock.Setup( repo => repo.GetAllAsync( It.IsAny<CancellationToken>( ) ) )
                               .ReturnsAsync( users );

            var userDtos = users.Take( 2 ).Select( u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role
            } ).ToList( );

            _mapperMock.Setup( m => m.Map<List<UserDto>>( It.IsAny<List<User>>( ) ) )
                       .Returns( ( List<User> src ) => src.Select( u => new UserDto
                       {
                           Id = u.Id,
                           Email = u.Email,
                           Phone = u.Phone,
                           Role = u.Role
                       } ).ToList( ) );

            var query = new GetAllUsersQuery { Page = 1, PageSize = 2 };

            // Act
            var result = await _handler.Handle( query, CancellationToken.None );

            // Assert
            result.ShouldNotBeNull( );
            result.Count.ShouldBe( 2 );
            result[ 0 ].Email.ShouldBe( "a@test.com" );
            result[ 1 ].Email.ShouldBe( "b@test.com" );
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Users( )
        {
            // Arrange
            _userRepositoryMock.Setup( repo => repo.GetAllAsync( It.IsAny<CancellationToken>( ) ) )
                               .ReturnsAsync( new List<User>( ) );

            _mapperMock.Setup( m => m.Map<List<UserDto>>( It.IsAny<List<User>>( ) ) )
                       .Returns( new List<UserDto>( ) );

            var query = new GetAllUsersQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle( query, CancellationToken.None );

            // Assert
            result.ShouldNotBeNull( );
            result.ShouldBeEmpty( );
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_Page_Out_Of_Range( )
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "x@test.com", Phone = "999", Role = RoleEnum.User }
            };

            _userRepositoryMock.Setup( repo => repo.GetAllAsync( It.IsAny<CancellationToken>( ) ) )
                               .ReturnsAsync( users );

            _mapperMock.Setup( m => m.Map<List<UserDto>>( It.IsAny<List<User>>( ) ) )
                       .Returns( new List<UserDto>( ) );

            var query = new GetAllUsersQuery { Page = 5, PageSize = 10 };

            // Act
            var result = await _handler.Handle( query, CancellationToken.None );

            // Assert
            result.ShouldNotBeNull( );
            result.ShouldBeEmpty( );
        }
    }
}
