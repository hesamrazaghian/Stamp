using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Infrastructure.Data;
using Stamp.Infrastructure.Repositories;
using Stamp.Infrastructure.Services;
using Stamp.Web.Middleware;
using System;
using Stamp.Application.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Stamp.Application.DTOs; // برای استفاده از کلاس JwtSettings

var builder = WebApplication.CreateBuilder( args );

#region Service Configuration

// ================== Controllers & API Exploration ==================
// Add controller services to handle API requests and enable API Explorer for Swagger.
builder.Services.AddControllers( );
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddScoped<IJwtService, JwtService>( );
builder.Services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( RegisterUserCommand ).Assembly ) );

// ================== Bind JWT Settings (Configuration to Strongly Typed Class) ==================
// Bind JwtSettings section from appsettings.json to JwtSettings class for dependency injection
var jwtSettingsSection = builder.Configuration.GetSection( "JwtSettings" );
builder.Services.Configure<JwtSettings>( jwtSettingsSection );

// ================== JWT Authentication ==================
// Configure JWT authentication scheme and token validation parameters
builder.Services
    .AddAuthentication( options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    } )
    .AddJwtBearer( options =>
    {
        options.RequireHttpsMetadata = true; // فقط HTTPS
        options.SaveToken = true;  // ذخیره توکن در context

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,   // بررسی صادرکننده توکن
            ValidateAudience = true, // بررسی مخاطب توکن
            ValidateIssuerSigningKey = true, // بررسی کلید امضا
            ValidIssuer = jwtSettingsSection[ "Issuer" ],   // مقادیر از فایل appsettings.json
            ValidAudience = jwtSettingsSection[ "Audience" ],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes( jwtSettingsSection[ "SecretKey" ]! )
            )
        };
    } );

// ================== Swagger / OpenAPI Configuration ==================
// Registers the Swagger generator with basic OpenAPI document metadata.
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc( "v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    } );
} );

// ================== Application Services ==================
// Dependency Injection for repositories and service implementations.
builder.Services.AddScoped<IUserRepository, UserRepository>( );
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>( );
builder.Services.AddAutoMapper( typeof( UserProfile ) );

// ================== Database Context Configuration ==================
// Configure Entity Framework Core with SQL Server as the database provider.
builder.Services.AddDbContext<ApplicationDbContext>( options =>
    options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) ) );

#endregion

var app = builder.Build( );

#region Middleware Pipeline

// ================== Global Exception Handling ==================
// Set the custom ExceptionHandlingMiddleware as the first middleware to handle all exceptions globally.
app.UseMiddleware<ExceptionHandlingMiddleware>( );

// ================== Swagger Middleware (Development Only) ==================
// Enable Swagger UI and JSON docs only when in development mode.
if( app.Environment.IsDevelopment( ) )
{
    app.UseSwagger( );
    app.UseSwaggerUI( c =>
    {
        c.SwaggerEndpoint( "/swagger/v1/swagger.json", "My API V1" );
    } );
}

// ================== Security & Routing Middleware ==================
// Redirect HTTP to HTTPS for secure communication.
app.UseHttpsRedirection( );

// ================== Authentication & Authorization Middleware ==================
// Enable authentication for validating JWT tokens
app.UseAuthentication( );

// Enable Authorization middleware to handle role-based or policy-based authorization
app.UseAuthorization( );

// Map controller endpoints to routes.
app.MapControllers( );

#endregion

app.Run( );
