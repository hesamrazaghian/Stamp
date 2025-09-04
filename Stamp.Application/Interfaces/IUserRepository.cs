using Stamp.Domain.Entities;
using Stamp.Domain.Enums;

namespace Stamp.Application.Interfaces
{
    /// <summary>
    /// Represents a contract for performing CRUD operations on User entity
    /// </summary>
    public interface IUserRepository
    {
        // CREATE -----------------------------

        /// <summary>
        /// Add a new user
        /// </summary>
        Task AddAsync( User user, CancellationToken cancellationToken );

        // READ --------------------------------

        /// <summary>
        /// Get a user by Id (only non-deleted)
        /// </summary>
        Task<User?> GetByIdAsync( Guid userId, CancellationToken cancellationToken );

        /// <summary>
        /// Get a user by Email (only non-deleted)
        /// </summary>
        Task<User?> GetByEmailAsync( string email, CancellationToken cancellationToken );

        /// <summary>
        /// Check if a user exists with specific email
        /// </summary>
        Task<bool> ExistsByEmailAsync( string email, CancellationToken cancellationToken );

        /// <summary>
        /// Get all non-deleted users
        /// </summary>
        Task<List<User>> GetAllAsync( CancellationToken cancellationToken );

        // UPDATE ------------------------------

        /// <summary>
        /// Update a user entity
        /// </summary>
        Task UpdateAsync( User user, CancellationToken cancellationToken );

        /// <summary>
        /// Update only a user's role (based on RoleEnum)
        /// </summary>
        Task UpdateUserRoleAsync( Guid userId, RoleEnum role, CancellationToken cancellationToken );

        // DELETE ------------------------------

        /// <summary>
        /// Soft delete a user (mark as deleted)
        /// </summary>
        Task SoftDeleteAsync( Guid id, CancellationToken cancellationToken );

        /// <summary>
        /// Hard delete a user (remove from database)
        /// </summary>
        Task HardDeleteAsync( Guid id, CancellationToken cancellationToken );
    }
}
