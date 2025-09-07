using MediatR;
using Stamp.Application.DTOs;
using System.Collections.Generic;

namespace Stamp.Application.Queries.Users
{

    // Query to retrieve all users from the system
    public class GetAllUsersQuery : IRequest<List<UserDto>>
    {
        // Page number for pagination (default: 1)
        public int Page { get; set; } = 1;

        // Number of items per page (default: 10)
        public int PageSize { get; set; } = 10;
    }
}
