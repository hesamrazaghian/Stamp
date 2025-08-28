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
        // فیلتر جهانی Tenant (در صورت استفاده از سرویس Tenant)
        if( _currentTenantService != null )
        {
            modelBuilder.Entity<User>( )
                .HasQueryFilter( u => u.UserTenants
                    .Any( ut => ut.TenantId == _currentTenantService.TenantId ) );
        }

        // Soft Delete Filter
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

        // User
        modelBuilder.Entity<User>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Email ).IsRequired( ).HasMaxLength( 256 );
            entity.Property( e => e.Phone ).HasMaxLength( 20 );
            entity.Property( e => e.PasswordHash ).IsRequired( );
            entity.Property( e => e.Role ).IsRequired( ).HasMaxLength( 50 );
            entity.HasIndex( e => new { e.Email, e.TenantId } ).IsUnique( );
        } );

        // Tenant
        modelBuilder.Entity<Tenant>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.Property( e => e.BusinessType ).HasMaxLength( 100 );
        } );

        // UserTenant
        modelBuilder.Entity<UserTenant>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( ut => ut.User )
                .WithMany( u => u.UserTenants )
                .HasForeignKey( ut => ut.UserId );
            entity.HasOne( ut => ut.Tenant )
                .WithMany( t => t.UserTenants )
                .HasForeignKey( ut => ut.TenantId )
                .OnDelete( DeleteBehavior.Restrict ); // جلوگیری از multiple cascade paths
            entity.Property( ut => ut.TotalStamps ).HasDefaultValue( 0 );
            entity.HasIndex( ut => new { ut.UserId, ut.TenantId } ).IsUnique( );
        } );

        // StampTransaction
        modelBuilder.Entity<StampTransaction>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Type ).IsRequired( ).HasMaxLength( 20 );
            entity.Property( e => e.Quantity ).IsRequired( );
            entity.HasOne( e => e.User ).WithMany( ).HasForeignKey( e => e.UserId );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict ); // جلوگیری از multiple cascade paths
        } );

        // Reward
        modelBuilder.Entity<Reward>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict ); // جلوگیری از multiple cascade paths
        } );

        // RewardRedemption
        modelBuilder.Entity<RewardRedemption>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( e => e.User ).WithMany( ).HasForeignKey( e => e.UserId );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict ); // جلوگیری از multiple cascade paths
            entity.HasOne( e => e.Reward ).WithMany( r => r.RewardRedemptions ).HasForeignKey( e => e.RewardId );
        } );

        // StampRule
        modelBuilder.Entity<StampRule>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.HasOne( e => e.Tenant ).WithMany( ).HasForeignKey( e => e.TenantId )
                .OnDelete( DeleteBehavior.Restrict ); // جلوگیری از multiple cascade paths
        } );

        base.OnModelCreating( modelBuilder );
    }
}
