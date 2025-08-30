using Microsoft.AspNetCore.Http;
using Stamp.Application.Interfaces;
using Stamp.Infrastructure.Data;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Middleware;

public class CurrentTenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly ApplicationDbContext _dbContext;

    public CurrentTenantMiddleware(
        RequestDelegate next,
        ICurrentTenantService currentTenantService,
        ApplicationDbContext dbContext )
    {
        _next = next;
        _currentTenantService = currentTenantService;
        _dbContext = dbContext;
    }

    public async Task InvokeAsync( HttpContext context )
    {
        var tenantId = _currentTenantService.GetCurrentTenantId( );
        _dbContext.CurrentTenantId = tenantId;

        await _next( context );
    }
}