using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stamp.Application.Commands.Users;
using Stamp.Application.DTOs;
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

        [HttpPost( "register" )]
        [ProducesResponseType( typeof( UserDto ), StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status400BadRequest )]
        public async Task<ActionResult<UserDto>> Register( [FromBody] RegisterUserCommand command )
        {
            return Ok( await _mediator.Send( command ) );
        }

        [HttpDelete( "{id:guid}" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> Delete( Guid id )
        {
            var result = await _mediator.Send( new DeleteUserCommand { UserId = id } );
            if( !result )
                return NotFound( );

            return Ok( new { message = "User deleted successfully" } );
        }

        [HttpGet]
        [ProducesResponseType( typeof( List<UserDto> ), StatusCodes.Status200OK )]
        public async Task<ActionResult<List<UserDto>>> GetAll( [FromQuery] int page = 1, [FromQuery] int pageSize = 10 )
        {
            // Build the query object for MediatR
            var query = new GetAllUsersQuery
            {
                Page        = page,
                PageSize    = pageSize
            };

            // Send the query to its handler
            var users = await _mediator.Send( query );

            // Return the paginated user list
            return Ok( users );
        }

    }
}
