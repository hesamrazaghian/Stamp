using AutoMapper;
using Stamp.Application.DTOs;
using Stamp.Domain.Entities;

namespace Stamp.Application.Mappings
{
    /// AutoMapper profile mapping between User entity and UserDto.
    public class UserProfile : Profile
    {
        public UserProfile( )
        {
            // Map from User entity to UserDto
            CreateMap<User, UserDto>( );

            // Optional: Map back from DTO to entity if needed
            // CreateMap<UserDto, User>();
        }
    }
}
