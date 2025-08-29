using AutoMapper;
using Stamp.Application.DTOs;
using Stamp.Domain.Entities;

namespace Stamp.Application.Mappings;

public class TenantProfile : Profile
{
    public TenantProfile( )
    {
        // ✅ فقط مپینگ‌های مربوط به Tenant اینجا قرار می‌گیرن
        CreateMap<Tenant, TenantDto>( );
    }
}