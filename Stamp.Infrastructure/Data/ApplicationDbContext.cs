using Microsoft.EntityFrameworkCore;
using Stamp.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace Stamp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    // ✅ این پراپرتی رو اضافه کن (حیاتی برای فیلتر جهانی)
    public Guid? CurrentTenantId { get; set; }

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
        // ✅ فیلتر جهانی برای Soft Delete (برای تمام موجودیت‌های دارای IsDeleted)
        foreach( var entityType in modelBuilder.Model.GetEntityTypes( ) )
        {
            if( typeof( BaseEntity ).IsAssignableFrom( entityType.ClrType ) &&
                entityType.ClrType.GetProperty( "IsDeleted" ) != null )
            {
                var parameter = Expression.Parameter( entityType.ClrType, "e" );
                var isDeletedProperty = Expression.Property( parameter, "IsDeleted" );
                var filter = Expression.Equal( isDeletedProperty, Expression.Constant( false ) );
                var lambda = Expression.Lambda( filter, parameter );
                modelBuilder.Entity( entityType.ClrType ).HasQueryFilter( lambda );
            }
        }

        // ✅ فیلتر جهانی برای TenantId (فقط برای موجودیت‌های دارای TenantId)
        ApplyTenantFilter<UserTenant>( modelBuilder );
        ApplyTenantFilter<StampTransaction>( modelBuilder );
        ApplyTenantFilter<Reward>( modelBuilder );
        ApplyTenantFilter<RewardRedemption>( modelBuilder );
        ApplyTenantFilter<StampRule>( modelBuilder );

        // تنظیمات موجودیت‌ها بدون تغییر
        ConfigureUserEntity( modelBuilder );
        ConfigureTenantEntity( modelBuilder );
        ConfigureUserTenantEntity( modelBuilder );
        ConfigureStampTransactionEntity( modelBuilder );
        ConfigureRewardEntity( modelBuilder );
        ConfigureRewardRedemptionEntity( modelBuilder );
        ConfigureStampRuleEntity( modelBuilder );

        base.OnModelCreating( modelBuilder );
    }

    private void ApplyTenantFilter<TEntity>( ModelBuilder modelBuilder ) where TEntity : class
    {
        var entityType = modelBuilder.Model.FindEntityType( typeof( Tenant ) );
        if( entityType != null )
        {
            var parameter = Expression.Parameter( typeof( TEntity ), "e" );
            var tenantIdProperty = Expression.Property( parameter, "TenantId" );
            var currentTenantId = Expression.Property(
                Expression.Constant( this ),
                nameof( CurrentTenantId )
            );

            // ✅ فیلتر: فقط رکوردهای مربوط به Tenant فعلی یا عمومی (برای Guest)
            var filter = Expression.OrElse(
                Expression.Equal( currentTenantId, Expression.Constant( null ) ),
                Expression.Equal( tenantIdProperty, currentTenantId )
            );

            var lambda = Expression.Lambda( filter, parameter );
            modelBuilder.Entity<TEntity>( ).HasQueryFilter( lambda );
        }
    }

    private void ConfigureUserEntity( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<User>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Email ).IsRequired( ).HasMaxLength( 256 );
            entity.Property( e => e.Phone ).HasMaxLength( 20 );
            entity.Property( e => e.PasswordHash ).IsRequired( );
            entity.Property( e => e.Role ).IsRequired( ).HasMaxLength( 50 );
            entity.HasIndex( e => new { e.Email, e.TenantId } ).IsUnique( );
        } );
    }

    private void ConfigureTenantEntity( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<Tenant>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.Property( e => e.BusinessType ).HasMaxLength( 100 );
        } );
    }

    private void ConfigureUserTenantEntity( ModelBuilder modelBuilder )
    {
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
    }

    private void ConfigureStampTransactionEntity( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<StampTransaction>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Type ).IsRequired( ).HasMaxLength( 20 );
            entity.Property( e => e.Quantity ).IsRequired( );
            entity.HasOne( e => e.User ).WithMany( ).HasForeignKey( e => e.UserId );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
        } );
    }

    private void ConfigureRewardEntity( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<Reward>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
        } );
    }

    private void ConfigureRewardRedemptionEntity( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<RewardRedemption>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( e => e.User ).WithMany( ).HasForeignKey( e => e.UserId );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
            entity.HasOne( e => e.Reward ).WithMany( r => r.RewardRedemptions ).HasForeignKey( e => e.RewardId );
        } );
    }

    private void ConfigureStampRuleEntity( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<StampRule>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict );
        } );
    }
}