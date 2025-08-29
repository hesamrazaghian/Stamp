using AutoMapper;
using MediatR;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Application.Queries.Tenants;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Handlers.Tenants;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, List<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public GetAllTenantsQueryHandler( ITenantRepository tenantRepository, IMapper mapper )
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public async Task<List<TenantDto>> Handle( GetAllTenantsQuery request, CancellationToken cancellationToken )
    {
        var tenants = await _tenantRepository.GetAllAsync( cancellationToken );
        return _mapper.Map<List<TenantDto>>( tenants );
    }
}