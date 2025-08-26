using Stamp.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync( User user, CancellationToken cancellationToken );
    Task<User?> GetByEmailAsync( string email, Guid tenantId, CancellationToken cancellationToken );
    Task<User?> GetByPhoneAsync( string phone, Guid tenantId, CancellationToken cancellationToken );
    Task<bool> ExistsByEmailAsync( string email, Guid tenantId, CancellationToken cancellationToken );
}