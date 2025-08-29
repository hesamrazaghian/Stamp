using Microsoft.EntityFrameworkCore;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using System.Linq.Expressions;

namespace Stamp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenantService? _currentTenantService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantService currentTenantService )
        : base( options )
    {
        _currentTenantService = currentTenantService;
    }

    public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options )
        : base( options )
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<UserTenant> UserTenants { get; set; } = null!;
    public DbSet<StampTransaction> StampTransactions { get; set; } = null!;
    public DbSet<Reward> Rewards { get; set; } = null!;
    public DbSet<RewardRedemption> RewardRedemptions { get; set; } = null!;
    public DbSet<StampRule> StampRules { get; set; } = null!;

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        // ✅ رفع خطای اصلی: جایگزینی فیلتر جهانی با روش صحیح
        // توضیح: در EF Core نباید مستقیماً روی کلاس پایه (BaseEntity) فیلتر اعمال کرد
        foreach( var entityType in modelBuilder.Model.GetEntityTypes( ) )
        {
            if( typeof( BaseEntity ).IsAssignableFrom( entityType.ClrType ) )
            {
                var parameter = Expression.Parameter( entityType.ClrType, "e" );

                // ✅ ساخت عبارت فیلتر برای Soft Delete
                var softDeleteFilter = Expression.Not(
                    Expression.Property( parameter, nameof( BaseEntity.IsDeleted ) )
                );

                // ✅ اگر سرویس Tenant موجود باشد، فیلتر TenantId هم اضافه شود
                if( _currentTenantService != null )
                {
                    var tenantFilter = Expression.Equal(
                        Expression.Property( parameter, nameof( BaseEntity.TenantId ) ),
                        Expression.Constant( _currentTenantService.TenantId )
                    );

                    // ✅ ترکیب فیلترها: TenantId AND NOT IsDeleted
                    var filterExpression = Expression.AndAlso( tenantFilter, softDeleteFilter );
                    var lambda = Expression.Lambda( filterExpression, parameter );
                    modelBuilder.Entity( entityType.ClrType ).HasQueryFilter( lambda );
                }
                else
                {
                    // ✅ فقط Soft Delete برای مایگریشن‌ها
                    var lambda = Expression.Lambda( softDeleteFilter, parameter );
                    modelBuilder.Entity( entityType.ClrType ).HasQueryFilter( lambda );
                }
            }
        }

        // User (بدون تغییر)
        modelBuilder.Entity<User>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Email ).IsRequired( ).HasMaxLength( 256 );
            entity.Property( e => e.Phone ).HasMaxLength( 20 );
            entity.Property( e => e.PasswordHash ).IsRequired( );
            entity.Property( e => e.Role ).IsRequired( ).HasMaxLength( 50 );
            entity.HasIndex( e => new { e.Email, e.TenantId } ).IsUnique( );
        } );

        // Tenant (بدون تغییر)
        modelBuilder.Entity<Tenant>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.Property( e => e.BusinessType ).HasMaxLength( 100 );
        } );

        // UserTenant (بدون تغییر)
        modelBuilder.Entity<UserTenant>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( ut => ut.User )
                .WithMany( u => u.UserTenants )
                .HasForeignKey( ut => ut.UserId );
            entity.HasOne( ut => ut.Tenant )
                .WithMany( t => t.UserTenants )
                .HasForeignKey( ut => ut.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
            entity.Property( ut => ut.TotalStamps ).HasDefaultValue( 0 );
            entity.HasIndex( ut => new { ut.UserId, ut.TenantId } ).IsUnique( );
        } );

        // StampTransaction (بدون تغییر)
        modelBuilder.Entity<StampTransaction>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Type ).IsRequired( ).HasMaxLength( 20 );
            entity.Property( e => e.Quantity ).IsRequired( );
            entity.HasOne( e => e.User ).WithMany( ).HasForeignKey( e => e.UserId );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
        } );

        // Reward (بدون تغییر)
        modelBuilder.Entity<Reward>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
        } );

        // RewardRedemption (بدون تغییر)
        modelBuilder.Entity<RewardRedemption>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( e => e.User ).WithMany( ).HasForeignKey( e => e.UserId );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
            entity.HasOne( e => e.Reward ).WithMany( r => r.RewardRedemptions ).HasForeignKey( e => e.RewardId );
        } );

        // StampRule (بدون تغییر)
        modelBuilder.Entity<StampRule>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
        } );

        base.OnModelCreating( modelBuilder );
    }
}