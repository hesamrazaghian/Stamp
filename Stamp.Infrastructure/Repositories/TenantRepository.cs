using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository( ApplicationDbContext context )
    {
        _context = context;
    }

    public async Task AddAsync( Tenant tenant, CancellationToken cancellationToken )
    {
        await _context.Tenants.AddAsync( tenant, cancellationToken );
    }

    public async Task<Tenant?> GetByIdAsync( Guid id, CancellationToken cancellationToken )
    {
        return await _context.Tenants.FindAsync( new object[ ] { id }, cancellationToken );
    }

    public async Task SaveChangesAsync( CancellationToken cancellationToken )
    {
        await _context.SaveChangesAsync( cancellationToken );
    }
}