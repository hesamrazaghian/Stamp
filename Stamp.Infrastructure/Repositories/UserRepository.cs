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
                .Include( u => u.UserTenants )
                .ThenInclude( ut => ut.Tenant )
                .FirstOrDefaultAsync( u =>
                    u.Email == email &&
                    !u.IsDeleted,
                    cancellationToken );
        }

        public async Task<User?> GetByEmailAndTenantAsync( string email, Guid? tenantId, CancellationToken cancellationToken )
        {
            if( tenantId.HasValue )
            {
                return await _context.Users
                    .Include( u => u.UserTenants )
                    .ThenInclude( ut => ut.Tenant )
                    .FirstOrDefaultAsync( u =>
                        u.Email == email &&
                        u.UserTenants.Any( ut => ut.TenantId == tenantId.Value && !ut.IsDeleted ),
                        cancellationToken );
            }
            else
            {
                return await _context.Users
                    .FirstOrDefaultAsync( u =>
                        u.Email == email &&
                        !u.IsDeleted,
                        cancellationToken );
            }
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

        public async Task AddToTenantAsync( Guid userId, Guid tenantId, CancellationToken cancellationToken )
        {
            var userTenant = new UserTenant
            {
                Id = Guid.NewGuid( ),
                UserId = userId,
                TenantId = tenantId,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserTenants.AddAsync( userTenant, cancellationToken );
            await _context.SaveChangesAsync( cancellationToken );
        }

        public async Task CreateWithTenantAsync( User user, Guid tenantId, CancellationToken cancellationToken )
        {
            var userTenant = new UserTenant
            {
                Id = Guid.NewGuid( ),
                UserId = user.Id,
                TenantId = tenantId,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync( user, cancellationToken );
            await _context.UserTenants.AddAsync( userTenant, cancellationToken );
            await _context.SaveChangesAsync( cancellationToken );
        }

        public async Task<User?> GetWithTenantsAsync( Guid userId, CancellationToken cancellationToken )
        {
            return await _context.Users
                .Include( u => u.UserTenants )
                .ThenInclude( ut => ut.Tenant )
                .FirstOrDefaultAsync( u => u.Id == userId, cancellationToken );
        }

        // ✅ این متد رو اضافه کن (بررسی وجود هرگونه عضویت)
        public async Task<bool> HasAnyTenantMembershipAsync(
            Guid userId,
            CancellationToken cancellationToken )
        {
            return await _context.UserTenants
                .AnyAsync( ut => ut.UserId == userId && !ut.IsDeleted, cancellationToken );
        }

        // ✅ این متد رو اضافه کن (به‌روزرسانی نقش کاربر)
        public async Task UpdateUserRoleAsync(
            Guid userId,
            string role,
            CancellationToken cancellationToken )
        {
            var user = await _context.Users.FindAsync( new object[ ] { userId }, cancellationToken );
            if( user != null )
            {
                user.Role = role;
                await _context.SaveChangesAsync( cancellationToken );
            }
        }

        // ✅ این متد رو اضافه کن (حیاتی برای رفع خطا)
        public async Task<User?> GetByIdAsync(
            Guid userId,
            CancellationToken cancellationToken )
        {
            return await _context.Users
                .Include( u => u.UserTenants )
                .ThenInclude( ut => ut.Tenant )
                .FirstOrDefaultAsync( u =>
                    u.Id == userId &&
                    !u.IsDeleted,
                    cancellationToken );
        }
    }
}