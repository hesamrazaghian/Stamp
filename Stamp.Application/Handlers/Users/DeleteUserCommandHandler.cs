using MediatR;
using Stamp.Application.Commands.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task<bool> Handle( DeleteUserCommand request, CancellationToken cancellationToken )
        {
            throw new NotImplementedException( );
        }
    }
}
