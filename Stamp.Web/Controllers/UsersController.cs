using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Stamp.Application.Queries.Users;
using Stamp.Application.Commands.UserTenants; // ✅ این خط رو اضافه کن

namespace Stamp.Web.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController( IMediator mediator )
        {
            _mediator = mediator;
        }

        /// <summary>
        /// ثبت‌نام کاربر جدید در یک Tenant
        /// </summary>
        [HttpPost( "register" )]
        public async Task<ActionResult<UserDto>> Register( [FromBody] RegisterUserCommand command )
        {
            var result = await _mediator.Send( command );
            return Ok( result );
        }

        /// <summary>
        /// ورود کاربر و دریافت JWT Token
        /// </summary>
        [HttpPost( "login" )]
        public async Task<ActionResult> Login( [FromBody] LoginUserCommand command )
        {
            var token = await _mediator.Send( command );
            return Ok( new { Token = token } );
        }

        /// <summary>
        /// دریافت پروفایل کاربر جاری
        /// </summary>
        [Authorize]
        [HttpGet( "me" )]
        public async Task<ActionResult<UserProfileDto>> GetMe( )
        {
            var userIdClaim = User.FindFirst( "UserId" )?.Value;
            if( string.IsNullOrEmpty( userIdClaim ) || !Guid.TryParse( userIdClaim, out var userId ) )
                return Unauthorized( );

            var result = await _mediator.Send( new GetCurrentUserQuery { UserId = userId } );
            return Ok( result );
        }

        // ✅ این endpoint رو اضافه کن (حیاتی برای عضویت در Tenant)
        [Authorize]
        [HttpPost( "join-tenant" )]
        public async Task<IActionResult> JoinTenant( [FromBody] JoinTenantCommand command )
        {
            var userIdClaim = User.FindFirst( "UserId" )?.Value;
            if( string.IsNullOrEmpty( userIdClaim ) || !Guid.TryParse( userIdClaim, out var userId ) )
            {
                return Unauthorized( );
            }

            command.UserId = userId;
            await _mediator.Send( command );
            return Ok( );
        }
    }
}