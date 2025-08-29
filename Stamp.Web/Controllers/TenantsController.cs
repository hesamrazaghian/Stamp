using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stamp.Application.Commands.Tenants;
using Stamp.Application.Queries.Tenants; // ✅ اضافه شده: برای GetAllTenantsQuery
using Stamp.Infrastructure.Data;

namespace Stamp.Web.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController( IMediator mediator )
        {
            _mediator = mediator;
        }

        // ✅ endpoint عمومی: دسترسی بدون احراز هویت
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTenants( )
        {
            var tenants = await _mediator.Send( new GetAllTenantsQuery( ) );
            return Ok( tenants );
        }

        // 🔹 ایجاد Tenant (فقط برای کاربران احراز هویت شده)
        [HttpPost]
        public async Task<IActionResult> CreateTenant( [FromBody] CreateTenantCommand command )
        {
            var tenant = await _mediator.Send( command );
            return CreatedAtAction( nameof( GetTenantData ), new { tenantId = tenant.Id }, tenant );
        }

        // 🔹 دریافت داده خاص یک Tenant (نیاز به احراز هویت و سیاست SameTenantOnly)
        [Authorize( Policy = "SameTenantOnly" )]
        [HttpGet( "{tenantId:guid}" )]
        public IActionResult GetTenantData( Guid tenantId )
        {
            return Ok( $"Tenant-specific data for {tenantId}" );
        }

        // 🔹 تست فیلترهای امنیتی (فقط برای کاربران احراز هویت شده)
        [Authorize( Policy = "SameTenantOnly" )]
        [HttpGet( "{tenantId:guid}/test-security" )]
        public IActionResult TestSecurityFilters( Guid tenantId )
        {
            var dbContext = HttpContext.RequestServices
                .GetRequiredService<ApplicationDbContext>( );

            var users = dbContext.Users.ToList( );
            var transactions = dbContext.StampTransactions.ToList( );

            return Ok( new
            {
                Message = "تست فیلترهای امنیتی موفقیت‌آمیز بود",
                TenantIdFromClaim = User.FindFirst( "TenantId" )?.Value,
                TenantIdFromRoute = tenantId,
                UsersCount = users.Count,
                TransactionsCount = transactions.Count
            } );
        }
    }
}