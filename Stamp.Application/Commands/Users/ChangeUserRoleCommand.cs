using MediatR;
using Stamp.Domain.Enums;

namespace Stamp.Application.Commands.Users
{
    /// <summary>
    /// Command to change a user's role.
    /// </summary>
    public class ChangeUserRoleCommand : IRequest<bool>
    {
        // The ID of the user whose role will be changed
        public Guid UserId { get; set; }

        // The new role to assign
        public RoleEnum NewRole { get; set; }
    }
}
