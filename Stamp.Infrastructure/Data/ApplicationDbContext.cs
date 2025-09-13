using Microsoft.EntityFrameworkCore;
using Stamp.Domain.Entities;

namespace Stamp.Infrastructure.Data
{
    /// <summary>Application DbContext containing Users, Businesses, and Customers.</summary>
    public class ApplicationDbContext : DbContext
    {
        #region Constructor
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options ) : base( options ) { }
        #endregion

        #region DbSets
        /// <summary>Users table in the database.</summary>
        public DbSet<User> Users { get; set; } = null!;
        /// <summary>Businesses table in the database.</summary>
        public DbSet<Business> Businesses { get; set; } = null!;
        /// <summary>Customers table in the database.</summary>
        public DbSet<Customer> Customers { get; set; } = null!;
        #endregion

        #region Model Configuration
        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            #region User Entity Configuration
            modelBuilder.Entity<User>( entity =>
            {
                entity.HasKey( e => e.Id );
                entity.Property( e => e.Email ).IsRequired( ).HasMaxLength( 256 );
                entity.Property( e => e.Phone ).HasMaxLength( 20 );
                entity.Property( e => e.PasswordHash ).IsRequired( );
                entity.Property( e => e.Role ).IsRequired( ).HasMaxLength( 50 );
            } );
            #endregion

            #region Business Entity Configuration
            modelBuilder.Entity<Business>( entity =>
            {
                entity.HasKey( e => e.Id );
                entity.Property( e => e.Name ).IsRequired( ).HasMaxLength( 200 );
                entity.Property( e => e.OwnerUserId ).IsRequired( );
                entity.Property( e => e.IsActive ).IsRequired( );
            } );
            #endregion

            #region Customer Entity Configuration
            modelBuilder.Entity<Customer>( entity =>
            {
                entity.HasKey( e => e.Id );
                entity.Property( e => e.BusinessId ).IsRequired( );
                entity.Property( e => e.FullName ).IsRequired( ).HasMaxLength( 200 );
                entity.Property( e => e.Phone ).IsRequired( ).HasMaxLength( 20 );
                entity.Property( e => e.Email ).HasMaxLength( 256 );
                entity.Property( e => e.DateOfBirth ).IsRequired( false );
                entity.Property( e => e.TotalPoints ).IsRequired( );
                entity.Property( e => e.IsActive ).IsRequired( );

                // Relationship: Customer belongs to a Business (One-to-Many)
                entity.HasOne<Business>( )
                      .WithMany( )
                      .HasForeignKey( e => e.BusinessId )
                      .OnDelete( DeleteBehavior.Cascade );
            } );
            #endregion

            #region Seed Initial Admin User
            modelBuilder.Entity<User>( ).HasData( new User
            {
                Id = Guid.Parse( "11111111-1111-1111-1111-111111111111" ),
                Email = "hesam.rq1366@gmail.com",
                Phone = "09125782204",
                // Password: Admin@123 (Hashed with BCrypt)
                PasswordHash = "$2a$11$rOpRlknEomwoXaAxGNpBxeI78k5P/wLu4z7ZoaCcpwiYJy9ATymuC",
                Role = Stamp.Domain.Enums.RoleEnum.Admin,
                CreatedAt = new DateTime( 2025, 09, 07, 0, 0, 0, DateTimeKind.Utc ),
                IsDeleted = false
            } );
            #endregion

            base.OnModelCreating( modelBuilder );
        }
        #endregion
    }
}
