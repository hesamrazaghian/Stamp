using Stamp.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Interfaces;

public interface ITenantRepository
{
    Task AddAsync( Tenant tenant, CancellationToken cancellationToken );
    Task<Tenant?> GetByIdAsync( Guid id, CancellationToken cancellationToken );
    Task SaveChangesAsync( CancellationToken cancellationToken );
    Task<List<Tenant>> GetAllAsync( CancellationToken cancellationToken );
}