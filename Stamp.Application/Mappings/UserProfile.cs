using AutoMapper;
using Stamp.Domain.Entities;
using Stamp.Application.DTOs;

namespace Stamp.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile( )
    {
        CreateMap<User, UserDto>( )
            .ForMember( dest => dest.Tenants, opt =>
                opt.MapFrom( src => src.UserTenants.Where( ut => !ut.IsDeleted ) ) );

        CreateMap<User, UserProfileDto>( );

    }
}