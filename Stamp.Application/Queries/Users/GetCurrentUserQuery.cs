using MediatR;
using Stamp.Application.DTOs;

namespace Stamp.Application.Queries.Users;

public class GetCurrentUserQuery : IRequest<UserProfileDto>
{
    public Guid UserId { get; set; }
}
