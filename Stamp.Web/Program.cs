using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Application.Mappings;
using Stamp.Application.Settings;
using Stamp.Infrastructure.Data;
using Stamp.Infrastructure.Repositories;
using Stamp.Infrastructure.Services;
using System.Text;

namespace Stamp.Web
{
    public class Program
    {
        public static void Main( string[ ] args )
        {
            var builder = WebApplication.CreateBuilder( args );

            // --- تنظیم DI برای لایه Application ---
            builder.Services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( RegisterUserCommand ).Assembly ) );
            builder.Services.AddValidatorsFromAssembly( typeof( RegisterUserCommand ).Assembly );
            builder.Services.AddFluentValidationAutoValidation( );           // فعال کردن ولیدیشن سمت سرور
            builder.Services.AddFluentValidationClientsideAdapters( );       // ولیدیشن سمت کلاینت
            builder.Services.AddAutoMapper( typeof( UserProfile ).Assembly );

            // --- تنظیم DI برای لایه Infrastructure ---
            builder.Services.AddDbContext<ApplicationDbContext>( options =>
                options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) ) );

            builder.Services.AddScoped<IUserRepository, UserRepository>( );
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>( );

            // --- احراز هویت JWT ---
            var jwtSettings = builder.Configuration.GetSection( "JwtSettings" ).Get<JwtSettings>( );
            var key = Encoding.UTF8.GetBytes( jwtSettings.Secret ?? throw new InvalidOperationException( "JWT Secret not configured" ) );

            builder.Services.AddSingleton( jwtSettings );
            builder.Services.AddScoped<IJwtService, JwtService>( );

            builder.Services.AddAuthentication( options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            } )
            .AddJwtBearer( options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey( key ),
                    ClockSkew = TimeSpan.Zero
                };
            } );

            builder.Services.AddAuthorization( );

            builder.Services.AddControllers( );

            var app = builder.Build( );

            // Configure the HTTP request pipeline
            app.UseHttpsRedirection( );

            app.UseAuthentication( );   // ✅ همیشه قبل از UseAuthorization
            app.UseAuthorization( );

            app.MapControllers( );

            app.Run( );
        }
    }
}
