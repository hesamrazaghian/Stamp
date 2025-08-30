using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stamp.Application.Interfaces;
using Stamp.Infrastructure.Data;
using Stamp.Infrastructure.Middleware;
using Stamp.Infrastructure.Services;
using System;

namespace Stamp.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration )
    {
        services.AddDbContext<ApplicationDbContext>( options =>
            options.UseSqlServer( configuration.GetConnectionString( "DefaultConnection" ) ) );

        services.AddHttpContextAccessor( );
        services.AddScoped<ICurrentTenantService, CurrentTenantService>( );
        services.AddScoped<CurrentTenantMiddleware>( );

        return services;
    }
}