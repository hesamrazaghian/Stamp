using AutoMapper;
using MediatR;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Application.Queries.Users;

namespace Stamp.Application.Handlers.Users
{
    // Handles the retrieval of all users with pagination.
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler( IUserRepository userRepository, IMapper mapper )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> Handle( GetAllUsersQuery request, CancellationToken cancellationToken )
        {
            var users       = await _userRepository.GetAllAsync( cancellationToken );
            var skip        = ( request.Page - 1 ) * request.PageSize;
            var pagedUsers  = users.Skip( skip ).Take( request.PageSize ).ToList( );
            var userDtos    = _mapper.Map<List<UserDto>>( pagedUsers );

            return userDtos;
        }
    }
}
