using Microsoft.EntityFrameworkCore;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using Stamp.Domain.Enums;
using Stamp.Infrastructure.Data;

namespace Stamp.Infrastructure.Repositories
{
    /// <summary>
    /// Implements IUserRepository for CRUD operations on User entity
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository( ApplicationDbContext context )
        {
            _context = context;
        }

        // ============ CREATE ============

        public async Task AddAsync( User user, CancellationToken cancellationToken )
        {
            await _context.Users.AddAsync( user, cancellationToken );
            await _context.SaveChangesAsync( cancellationToken );
        }

        // ============== READ ==============

        public async Task<User?> GetByIdAsync( Guid userId, CancellationToken cancellationToken )
        {
            return await _context.Users
                .FirstOrDefaultAsync( u => u.Id == userId && !u.IsDeleted, cancellationToken );
        }

        public async Task<User?> GetByEmailAsync( string email, CancellationToken cancellationToken )
        {
            return await _context.Users
                .FirstOrDefaultAsync( u => u.Email == email && !u.IsDeleted, cancellationToken );
        }

        public async Task<bool> ExistsByEmailAsync( string email, CancellationToken cancellationToken )
        {
            return await _context.Users
                .AnyAsync( u => u.Email == email && !u.IsDeleted, cancellationToken );
        }

        public async Task<List<User>> GetAllAsync( CancellationToken cancellationToken )
        {
            return await _context.Users
                .Where( u => !u.IsDeleted )
                .ToListAsync( cancellationToken );
        }

        // ============ UPDATE ============

        public async Task UpdateAsync( User user, CancellationToken cancellationToken )
        {
            _context.Users.Update( user );
            await _context.SaveChangesAsync( cancellationToken );
        }

        public async Task UpdateUserRoleAsync( Guid userId, RoleEnum role, CancellationToken cancellationToken )
        {
            var user = await _context.Users.FirstOrDefaultAsync( u => u.Id == userId && !u.IsDeleted, cancellationToken );
            if( user != null )
            {
                user.Role = role;
                await _context.SaveChangesAsync( cancellationToken );
            }
        }

        // ============ DELETE ============

        public async Task SoftDeleteAsync( Guid id, CancellationToken cancellationToken )
        {
            var user = await _context.Users.FirstOrDefaultAsync( u => u.Id == id && !u.IsDeleted, cancellationToken );
            if( user != null )
            {
                user.IsDeleted = true;
                await _context.SaveChangesAsync( cancellationToken );
            }
        }

        public async Task HardDeleteAsync( Guid id, CancellationToken cancellationToken )
        {
            var user = await _context.Users.FirstOrDefaultAsync( u => u.Id == id, cancellationToken );
            if( user != null )
            {
                _context.Users.Remove( user );
                await _context.SaveChangesAsync( cancellationToken );
            }
        }
    }
}
