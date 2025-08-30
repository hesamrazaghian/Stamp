using AutoMapper;
using Stamp.Domain.Entities;
using Stamp.Application.DTOs;

namespace Stamp.Application.Mappings;

public class TenantProfile : Profile
{
    public TenantProfile( )
    {
        CreateMap<Tenant, TenantDto>( );
        CreateMap<Tenant, TenantSummaryDto>( );
        CreateMap<Tenant, TenantListItemDto>( )
            .ForMember( dest => dest.TotalUsers, opt =>
                opt.MapFrom( src => src.UserTenants.Count( ut => !ut.IsDeleted ) ) );
    }
}