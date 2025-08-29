using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stamp.Web.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class AdminController : ControllerBase
    {
        [Authorize( Policy = "RequireAdminRole" )]
        [HttpGet( "admin-only" )]
        public IActionResult AdminOnlyData( )
        {
            return Ok( "This is admin data" );
        }
    }
}
