using Microsoft.EntityFrameworkCore;
using Stamp.Domain.Entities;

namespace Stamp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options ) : base( options )
    {
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<UserTenant> UserTenants { get; set; } = null!;


    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        // تنظیمات Entity User
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

            entity.HasQueryFilter( e => !e.IsDeleted );
        } );

        modelBuilder.Entity<Tenant>( entity =>
        {
            entity.HasKey( e => e.Id );
            entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
            entity.Property( e => e.BusinessType ).HasMaxLength( 100 );
            entity.HasQueryFilter( e => !e.IsDeleted );
        } );

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
            entity.HasQueryFilter( e => !e.IsDeleted );
        } );



        base.OnModelCreating( modelBuilder );
    }
}