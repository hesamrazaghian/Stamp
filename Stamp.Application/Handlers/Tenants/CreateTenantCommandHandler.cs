using AutoMapper;
using MediatR;
using Stamp.Application.Commands.Tenants;
using Stamp.Application.Interfaces;
using Stamp.Application.Mappings;
using Stamp.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Stamp.Application.DTOs;

namespace Stamp.Application.Handlers.Tenants;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public CreateTenantCommandHandler(
        ITenantRepository tenantRepository,
        IMapper mapper )
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public async Task<TenantDto> Handle(
        CreateTenantCommand command,
        CancellationToken cancellationToken )
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid( ),
            Name = command.Name,
            BusinessType = command.BusinessType
        };

        await _tenantRepository.AddAsync( tenant, cancellationToken );
        await _tenantRepository.SaveChangesAsync( cancellationToken );

        return _mapper.Map<TenantDto>( tenant );
    }
}