using Microsoft.EntityFrameworkCore;
using Stamp.Domain.Entities;

namespace Stamp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options )
            : base( options )
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
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
            } );

            base.OnModelCreating( modelBuilder );
        }
    }
}
