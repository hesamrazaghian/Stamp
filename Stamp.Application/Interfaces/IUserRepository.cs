using Stamp.Domain.Entities;

namespace Stamp.Application.Interfaces
{
    public interface IUserRepository
    {
        // اضافه کردن کاربر جدید
        Task AddAsync( User user, CancellationToken cancellationToken );

        // گرفتن کاربر فقط بر اساس ایمیل (سراسری)
        Task<User?> GetByEmailAsync( string email, CancellationToken cancellationToken );

        // گرفتن کاربر همراه Tenant خاص
        Task<User?> GetByEmailAndTenantAsync( string email, Guid tenantId, CancellationToken cancellationToken );

        // آیا ایمیل کاربر در کل سیستم وجود دارد؟
        Task<bool> ExistsByEmailAsync( string email, CancellationToken cancellationToken );

        // آیا این کاربر در Tenant خاصی عضویت دارد؟
        Task<bool> ExistsInTenantAsync( Guid userId, Guid tenantId, CancellationToken cancellationToken );
    }
}
