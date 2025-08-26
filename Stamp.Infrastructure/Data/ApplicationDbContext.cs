using Microsoft.EntityFrameworkCore;
using Stamp.Domain.Entities;

namespace Stamp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options ) : base( options )
    {
    }
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        // تنظیمات Entity User
        modelBuilder.Entity<User>( entity =>
        {
            // کلید اصلی
            entity.HasKey( e => e.Id );

            // فیلدهای اجباری
            entity.Property( e => e.Email )
                  .IsRequired( )
                  .HasMaxLength( 256 );

            entity.Property( e => e.Phone )
                  .HasMaxLength( 20 );

            entity.Property( e => e.PasswordHash )
                  .IsRequired( );

            entity.Property( e => e.Role )
                  .IsRequired( )
                  .HasMaxLength( 50 );

            entity.Property( e => e.TenantId )
                  .IsRequired( );

            // ایندکس برای ایمیل — برای جستجوی سریع
            entity.HasIndex( e => new { e.Email, e.TenantId } ).IsUnique( );

            // فیلتر عمومی: فقط کاربرانی که حذف نشدن رو نشون بده
            entity.HasQueryFilter( e => !e.IsDeleted );
        } );

        base.OnModelCreating( modelBuilder );
    }
}