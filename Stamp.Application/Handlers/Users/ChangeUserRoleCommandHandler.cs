using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;

namespace Stamp.Application.Handlers.Users
{
    /// <summary>
    /// Handles the role change of a user.
    /// </summary>
    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, bool>
    {
        #region Fields

        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructor

        public ChangeUserRoleCommandHandler( IUserRepository userRepository )
        {
            _userRepository = userRepository;
        }

        #endregion

        #region Handle Method

        public async Task<bool> Handle( ChangeUserRoleCommand request, CancellationToken cancellationToken )
        {
            // Check if the user exists
            var user = await _userRepository.GetByIdAsync( request.UserId, cancellationToken );
            if( user == null )
                return false;

            // Update user's role
            await _userRepository.UpdateUserRoleAsync( request.UserId, request.NewRole, cancellationToken );
            return true;
        }

        #endregion
    }
}
