using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stamp.Application.Commands.Tenants;
using Stamp.Infrastructure.Data; // ✅ این خط رو اضافه کن

namespace Stamp.Web.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator; // ✅ این خط رو اضافه کن

        public TenantsController( IMediator mediator ) // ✅ این خط رو اضافه کن
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant( [FromBody] CreateTenantCommand command )
        {
            var tenant = await _mediator.Send( command ); // ✅ این خط رو اضافه کن
            return CreatedAtAction( nameof( GetTenantData ), new { tenantId = tenant.Id }, tenant );
        }

        // 🔹 کد قبلی شما (اصلاً تغییر نمی‌دهیم)
        [Authorize( Policy = "SameTenantOnly" )]
        [HttpGet( "{tenantId:guid}" )]
        public IActionResult GetTenantData( Guid tenantId )
        {
            return Ok( $"Tenant-specific data for {tenantId}" );
        }

        // 🔹 این بخش رو جدید اضافه می‌کنیم (فقط برای تست امنیت)
        [Authorize( Policy = "SameTenantOnly" )]
        [HttpGet( "{tenantId:guid}/test-security" )]
        public IActionResult TestSecurityFilters( Guid tenantId )
        {
            // ✅ تست فیلتر TenantId (فقط داده‌های Tenant جاری باید برگردند)
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