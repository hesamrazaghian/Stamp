using AutoMapper;
using Stamp.Domain.Entities;
using Stamp.Application.DTOs;

namespace Stamp.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile( )
    {
        CreateMap<User, UserDto>( )
            .ForMember( dest => dest.Id, opt => opt.MapFrom( src => src.Id ) )
            .ForMember( dest => dest.Email, opt => opt.MapFrom( src => src.Email ) )
            .ForMember( dest => dest.Phone, opt => opt.MapFrom( src => src.Phone ) )
            .ForMember( dest => dest.Role, opt => opt.MapFrom( src => src.Role ) )
            .ForMember( dest => dest.CreatedAt, opt => opt.MapFrom( src => src.CreatedAt ) );
    }
}