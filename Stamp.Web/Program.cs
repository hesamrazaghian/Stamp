using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Infrastructure.Data;
using Stamp.Infrastructure.Repositories;
using Stamp.Infrastructure.Services;
using Stamp.Web.Middleware;
using System;

var builder = WebApplication.CreateBuilder( args );

#region Service Configuration

// ================== Controllers & API Exploration ==================
// Add controller services to handle API requests and enable API Explorer for Swagger.
builder.Services.AddControllers( );
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddScoped<IJwtService, JwtService>( );
builder.Services.AddMediatR( cfg => cfg.RegisterServicesFromAssembly( typeof( RegisterUserCommand ).Assembly ) );

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

// Enable Authorization middleware (requires authentication configuration in future).
app.UseAuthorization( );

// Map controller endpoints to routes.
app.MapControllers( );

#endregion

app.Run( );
