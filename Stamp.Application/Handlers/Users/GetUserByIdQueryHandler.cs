using AutoMapper;
using MediatR;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Application.Queries.Users;

namespace Stamp.Application.Handlers.Users
{
    // Handler to get a single user by Id using repository and AutoMapper
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler( IUserRepository userRepository, IMapper mapper )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle( GetUserByIdQuery request, CancellationToken cancellationToken )
        {
            // ✅ 1. Validate input
            if( request.UserId == Guid.Empty )
                throw new ArgumentException( "UserId cannot be empty.", nameof( request.UserId ) );

            // ✅ 2. Fetch user from repository
            var user = await _userRepository.GetByIdAsync( request.UserId, cancellationToken );

            // ✅ 3. Return null if not found, otherwise map to UserDto
            return user == null ? null : _mapper.Map<UserDto>( user );
        }
    }
}
