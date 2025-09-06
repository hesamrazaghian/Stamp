using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;

namespace Stamp.Application.Handlers.Users
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler( IUserRepository userRepository )
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle( DeleteUserCommand request, CancellationToken cancellationToken )
        {
            var user = await _userRepository.GetByIdAsync( request.UserId, cancellationToken );
            if( user == null )
                return false;

            await _userRepository.SoftDeleteAsync( request.UserId, cancellationToken );
            return true;
        }
    }
}
