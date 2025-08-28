using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Stamp.Application.Queries.Users;

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
            // اعتبارسنجی مدل (FluentValidation هم اینجا اعمال میشه)
            if( !ModelState.IsValid )
                return BadRequest( ModelState );

            var result = await _mediator.Send( command );

            return Ok( result );
        }

        /// <summary>
        /// ورود کاربر و دریافت JWT Token
        /// </summary>
        [HttpPost( "login" )]
        public async Task<ActionResult<string>> Login( [FromBody] LoginUserCommand command )
        {
            if( !ModelState.IsValid )
                return BadRequest( ModelState );

            var token = await _mediator.Send( command );
            return Ok( token );
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


    }
}
