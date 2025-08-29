using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stamp.Web.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class TenantsController : ControllerBase
    {
        [Authorize( Policy = "SameTenantOnly" )]
        [HttpGet( "{tenantId:guid}" )]
        public IActionResult GetTenantData( Guid tenantId )
        {
            return Ok( $"Tenant-specific data for {tenantId}" );
        }
    }
}
