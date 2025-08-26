using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Stamp.Infrastructure.Data;
using System.IO;

namespace Stamp.Infrastructure.Data.Factories
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext( string[ ] args )
        {
            // مسیر پروژه وب برای پیدا کردن appsettings.json
            var basePath = Path.Combine( Directory.GetCurrentDirectory( ), "../Stamp.Web" );

            var configuration = new ConfigurationBuilder( )
                .SetBasePath( basePath )
                .AddJsonFile( "appsettings.json", optional: false )
                .Build( );

            var connectionString = configuration.GetConnectionString( "DefaultConnection" );

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>( );
            optionsBuilder.UseSqlServer( connectionString );

            return new ApplicationDbContext( optionsBuilder.Options );
        }
    }
}
