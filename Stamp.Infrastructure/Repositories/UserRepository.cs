using Microsoft.EntityFrameworkCore;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Infrastructure.Data;

namespace Stamp.Infrastructure.Repositories
{
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

        public async Task<User?> GetByEmailAsync( string email, CancellationToken cancellationToken )
        {
            return await _context.Users
                .FirstOrDefaultAsync( u => u.Email == email, cancellationToken );
        }

        public async Task<User?> GetByEmailAndTenantAsync( string email, Guid tenantId, CancellationToken cancellationToken )
        {
            return await _context.Users
                .Include( u => u.UserTenants )
                .ThenInclude( ut => ut.Tenant )
                .FirstOrDefaultAsync( u =>
                    u.Email == email &&
                    u.UserTenants.Any( ut => ut.TenantId == tenantId && !ut.IsDeleted ),
                    cancellationToken );
        }

        public async Task<bool> ExistsByEmailAsync( string email, CancellationToken cancellationToken )
        {
            return await _context.Users
                .AnyAsync( u => u.Email == email, cancellationToken );
        }

        public async Task<bool> ExistsInTenantAsync( Guid userId, Guid tenantId, CancellationToken cancellationToken )
        {
            return await _context.UserTenants
                .AnyAsync( ut => ut.UserId == userId && ut.TenantId == tenantId && !ut.IsDeleted, cancellationToken );
        }
    }
}
