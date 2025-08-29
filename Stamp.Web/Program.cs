using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stamp.Application.Authorization;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Application.Mappings;
using Stamp.Application.Settings;
using Stamp.Infrastructure.Authorization;
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

            #region ⚙️ تنظیمات اولیه سیستم (حیاتی برای DI)
            // این سرویس برای دسترسی به HttpContext در لایه‌های پایین‌تر لازم است
            // حتماً باید اولین سرویس ثبت شود چون سایر سرویس‌ها به آن وابسته‌اند
            builder.Services.AddHttpContextAccessor( );
            #endregion

            #region 📦 تنظیم DI برای لایه Application (منطق کسب‌وکار)
            // ثبت MediatR برای پیاده‌سازی الگوی CQRS
            builder.Services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( RegisterUserCommand ).Assembly ) );

            // ثبت ولیدیشن‌ها با FluentValidation
            builder.Services.AddValidatorsFromAssembly( typeof( RegisterUserCommand ).Assembly );
            builder.Services.AddFluentValidationAutoValidation( );           // فعال‌سازی ولیدیشن سمت سرور
            builder.Services.AddFluentValidationClientsideAdapters( );       // فعال‌سازی ولیدیشن سمت کلاینت

            // ثبت AutoMapper برای مپینگ DTOها
            builder.Services.AddAutoMapper( typeof( UserProfile ).Assembly );
            #endregion

            #region 🗃️ تنظیم DI برای لایه Infrastructure (زیرساخت)
            // اتصال به دیتابیس SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>( options =>
                options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) ) );

            // ثبت ریپازیتوری‌ها و سرویس‌های زیرساخت
            builder.Services.AddScoped<IUserRepository, UserRepository>( );
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>( );

            // ثبت سرویس Tenant جاری برای استفاده در فیلترهای جهانی
            builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>( );
            #endregion

            #region 🔐 احراز هویت JWT (تنظیمات امنیتی حیاتی)
            // بارگذاری تنظیمات JWT از appsettings.json
            var jwtSettings = builder.Configuration
                .GetSection( "JwtSettings" )
                .Get<JwtSettings>( )
                ?? throw new InvalidOperationException( "JwtSettings section not found in configuration" );

            // بررسی صحت تنظیمات امنیتی
            if( string.IsNullOrWhiteSpace( jwtSettings.Secret ) )
                throw new InvalidOperationException( "JWT Secret is not configured" );

            // تبدیل کلید امنیتی به فرمت قابل استفاده
            var key = Encoding.UTF8.GetBytes( jwtSettings.Secret );

            // ثبت تنظیمات JWT در سیستم
            builder.Services.AddSingleton( jwtSettings );
            builder.Services.AddScoped<IJwtService, JwtService>( );
            #endregion

            #region 🛡️ تنظیمات احراز هویت و مجوزدهی
            // پیکربندی سیستم احراز هویت با JWT
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

            // تعریف سیاست‌های امنیتی سفارشی
            builder.Services.AddAuthorization( options =>
            {
                // ✅ سیاست ادمین: فقط کاربران با نقش Admin
                options.AddPolicy( "RequireAdminRole", policy =>
                    policy.RequireRole( "Admin" ) );

                // ✅ سیاست Tenant: فقط کاربران با TenantId مطابق منبع درخواستی
                options.AddPolicy( "SameTenantOnly", policy =>
                    policy.Requirements.Add( new SameTenantRequirement( ) ) );
            } );

            // ثبت Handler برای سیاست Tenant
            builder.Services.AddScoped<IAuthorizationHandler, SameTenantHandler>( );
            #endregion

            #region 🌐 تنظیمات API و ابزارهای توسعه
            // فعال‌سازی اکتشاف انتهای‌پوینت‌ها
            builder.Services.AddEndpointsApiExplorer( );

            // فعال‌سازی Swagger برای مستندسازی API
            builder.Services.AddSwaggerGen( );

            // ثبت کنترلرها
            builder.Services.AddControllers( );
            #endregion

            var app = builder.Build( );

            #region 🌐 پیکربندی Pipeline درخواست‌ها
            // فعال‌سازی Swagger فقط در محیط توسعه
            if( app.Environment.IsDevelopment( ) )
            {
                app.UseSwagger( );
                app.UseSwaggerUI( );
            }

            // تبدیل تمام درخواست‌ها به HTTPS
            app.UseHttpsRedirection( );

            // اجرای میدلورهای امنیتی (ترتیب حیاتی است)
            app.UseAuthentication( );   // ✅ همیشه قبل از UseAuthorization
            app.UseAuthorization( );

            // مپ کردن کنترلرها به روت‌ها
            app.MapControllers( );
            #endregion

            app.Run( );
        }
    }
}