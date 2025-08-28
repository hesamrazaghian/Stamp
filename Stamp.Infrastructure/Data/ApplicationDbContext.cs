using Microsoft.EntityFrameworkCore;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using System.Linq.Expressions;

namespace Stamp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenantService? _currentTenantService;

    public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenantService )
        : base( options )
    {
        _currentTenantService = currentTenantService;
    }

    // کانستراکتور برای Migration و Design‑Time
    public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options )
        : base( options )
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<UserTenant> UserTenants { get; set; } = null!;

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        // ✅ فیلتر TenantId فقط وقتی سرویس Tenant مشخصه
        if( _currentTenantService != null )
        {
            modelBuilder.Entity<User>( )
                .HasQueryFilter( u => u.UserTenants
                    .Any( ut => ut.TenantId == _currentTenantService.TenantId ) );
        }

        // ✅ فیلتر جهانی Soft Delete برای همه موجودیت‌هایی که از BaseEntity ارث‌بری کرده‌اند
        foreach( var entityType in modelBuilder.Model.GetEntityTypes( ) )
        {
            if( typeof( BaseEntity ).IsAssignableFrom( entityType.ClrType ) )
            {
                var parameter = Expression.Parameter( entityType.ClrType, "e" );
                var body = Expression.Equal(
                    Expression.Property( parameter, nameof( BaseEntity.IsDeleted ) ),
                    Expression.Constant( false )
                );
                var lambda = Expression.Lambda( body, parameter );
                modelBuilder.Entity( entityType.ClrType ).HasQueryFilter( lambda );
            }
        }

        // تنظیمات User
        modelBuilder.Entity<User>( entity =>
        {
            entity.HasKey( e => e.Id );

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

            entity.HasIndex( e => new { e.Email, e.TenantId } ).IsUnique( );
        } );

        // تنظیمات Tenant
        modelBuilder.Entity<Tenant>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.Property( e => e.BusinessType ).HasMaxLength( 100 );
        } );

        // تنظیمات UserTenant
        modelBuilder.Entity<UserTenant>( entity =>
        {
            entity.HasKey( e => e.Id );

            entity.HasOne( ut => ut.User )
                .WithMany( u => u.UserTenants )
                .HasForeignKey( ut => ut.UserId );

            entity.HasOne( ut => ut.Tenant )
                .WithMany( t => t.UserTenants )
                .HasForeignKey( ut => ut.TenantId );

            entity.HasIndex( ut => new { ut.UserId, ut.TenantId } ).IsUnique( );
        } );

        base.OnModelCreating( modelBuilder );
    }

}
