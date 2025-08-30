using AutoMapper;
using Stamp.Domain.Entities;
using Stamp.Application.DTOs;

namespace Stamp.Application.Mappings;

public class UserTenantProfile : Profile
{
    public UserTenantProfile( )
    {
        CreateMap<UserTenant, UserTenantDto>( )
            .ForMember( dest => dest.TenantName, opt =>
                opt.MapFrom( src => src.Tenant.Name ) )
            .ForMember( dest => dest.UserId, opt =>
                opt.MapFrom( src => src.User.Id ) )
            .ForMember( dest => dest.Email, opt =>
                opt.MapFrom( src => src.User.Email ) );
    }
}