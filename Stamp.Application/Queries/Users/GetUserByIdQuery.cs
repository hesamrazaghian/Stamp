using MediatR;
using Stamp.Application.DTOs;

namespace Stamp.Application.Queries.Users
{
    // Query to get a single user by Id
    public class GetUserByIdQuery : IRequest<UserDto?>
    {
        public Guid UserId { get; set; }
    }
}
