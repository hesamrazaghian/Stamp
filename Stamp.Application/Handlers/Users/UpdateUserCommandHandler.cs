using AutoMapper;
using MediatR;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;

namespace Stamp.Application.Handlers.Users
{
    /// <summary>
    /// Handles updating an existing user's details.
    /// </summary>
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler( IUserRepository userRepository, IMapper mapper )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle( UpdateUserCommand request, CancellationToken cancellationToken )
        {
            // Find the existing user
            var user = await _userRepository.GetByIdAsync( request.UserId, cancellationToken );
            if( user == null )
                return null;

            // Update properties (only overwrite if provided)
            if( !string.IsNullOrWhiteSpace( request.Email ) )
                user.Email = request.Email;

            if( !string.IsNullOrWhiteSpace( request.Phone ) )
                user.Phone = request.Phone;

            user.Role = request.Role; // Role update (we could optionally validate it)

            // Save changes
            await _userRepository.UpdateAsync( user, cancellationToken );

            // Map updated user to DTO
            return _mapper.Map<UserDto>( user );
        }
    }
}
