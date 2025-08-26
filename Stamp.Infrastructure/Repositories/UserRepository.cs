using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository( ApplicationDbContext context )
    {
        _context = context;
    }

    public async Task AddAsync( User user, CancellationToken cancellationToken )
    {
        await _context.Users.AddAsync( user, cancellationToken );
        await _context.SaveChangesAsync( cancellationToken );
    }

    public async Task<User?> GetByEmailAsync( string email, Guid tenantId, CancellationToken cancellationToken )
    {
        return await _context.Users
            .FirstOrDefaultAsync( u => u.Email == email && u.TenantId == tenantId, cancellationToken );
    }

    public async Task<User?> GetByPhoneAsync( string phone, Guid tenantId, CancellationToken cancellationToken )
    {
        return await _context.Users
            .FirstOrDefaultAsync( u => u.Phone == phone && u.TenantId == tenantId, cancellationToken );
    }

    public async Task<bool> ExistsByEmailAsync( string email, Guid tenantId, CancellationToken cancellationToken )
    {
        return await _context.Users
            .AnyAsync( u => u.Email == email && u.TenantId == tenantId, cancellationToken );
    }
}