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

        #region DbSets
        /// Users table in the database
        public DbSet<User> Users { get; set; } = null!;
        #endregion

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            #region User Entity Configuration
            // Configure properties for the User entity
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
            #endregion

            #region Seed Initial Admin User
            // Seed the database with the initial Admin user.
            // IMPORTANT: The PasswordHash below is a BCrypt hash for a predefined password.
            modelBuilder.Entity<User>( ).HasData( new User
            {
                Id = Guid.Parse( "11111111-1111-1111-1111-111111111111" ),
                Email = "hesam.rq1366@gmail.com",
                Phone = "09125782204",
                // Password: Admin@123  (Hashed with BCrypt)
                PasswordHash = "$2a$11$rOpRlknEomwoXaAxGNpBxeI78k5P/wLu4z7ZoaCcpwiYJy9ATymuC",
                Role = Stamp.Domain.Enums.RoleEnum.Admin,
                CreatedAt = new DateTime( 2025, 09, 07, 0, 0, 0, DateTimeKind.Utc ),
                IsDeleted = false
            } );
            #endregion

            base.OnModelCreating( modelBuilder );
        }
    }
}
