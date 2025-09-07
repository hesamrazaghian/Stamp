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
        #region Fields

        // MediatR instance for sending commands and queries
        private readonly IMediator _mediator;

        #endregion

        #region Constructor

        public UsersController( IMediator mediator )
        {
            _mediator = mediator;
        }

        #endregion

        // ===================================
        // ============== CREATE =============
        // ===================================

        #region Create Methods

        /// <summary>
        /// Register a new user in the system.
        /// </summary>
        [HttpPost( "register" )]
        [ProducesResponseType( typeof( UserDto ), StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status400BadRequest )]
        public async Task<ActionResult<UserDto>> Register( [FromBody] RegisterUserCommand command )
        {
            // Send the register command to MediatR handler
            var createdUser = await _mediator.Send( command );

            // Return the created user data
            return Ok( createdUser );
        }

        #endregion

        // ===================================
        // =============== READ ==============
        // ===================================

        #region Read Methods

        /// <summary>
        /// Get all users with pagination.
        /// </summary>
        [HttpGet]
        [ProducesResponseType( typeof( List<UserDto> ), StatusCodes.Status200OK )]
        public async Task<ActionResult<List<UserDto>>> GetAll( [FromQuery] int page = 1, [FromQuery] int pageSize = 10 )
        {
            // Build the query object for MediatR
            var query = new GetAllUsersQuery
            {
                Page = page,
                PageSize = pageSize
            };

            // Send the query to its handler
            var users = await _mediator.Send( query );

            // Return the paginated user list
            return Ok( users );
        }

        /// <summary>
        /// Get a single user by Id.
        /// </summary>
        [HttpGet( "{id:guid}" )]
        [ProducesResponseType( typeof( UserDto ), StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<ActionResult<UserDto>> GetById( Guid id )
        {
            // Send the query to get the user by Id
            var user = await _mediator.Send( new GetUserByIdQuery { UserId = id } );

            // If the user does not exist, return 404
            if( user == null )
                return NotFound( );

            // Return the found user
            return Ok( user );
        }

        #endregion

        // ===================================
        // ============== UPDATE =============
        // ===================================

        #region Update Methods

        /// <summary>
        /// Update user details by Id.
        /// </summary>
        [HttpPut( "{id:guid}" )]
        [ProducesResponseType( typeof( UserDto ), StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<ActionResult<UserDto>> Update( Guid id, [FromBody] UpdateUserCommand command )
        {
            // Ensure route Id matches the request model Id
            command.UserId = id;

            // Send the update command to the handler
            var updatedUser = await _mediator.Send( command );

            // If user not found, return 404
            if( updatedUser == null )
                return NotFound( );

            // Return the updated user
            return Ok( updatedUser );
        }

        /// <summary>
        /// Change a user's role by Id.
        /// </summary>
        [HttpPatch( "{id:guid}/role" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> ChangeUserRole( Guid id, [FromBody] ChangeUserRoleCommand command )
        {
            // Ensure route ID matches body ID
            command.UserId = id;

            // Send role change command to handler
            var success = await _mediator.Send( command );

            // If user not found, return 404
            if( !success )
                return NotFound( );

            // Return success confirmation
            return Ok( new { message = "User role updated successfully" } );
        }


        #endregion

        // ===================================
        // ============== DELETE =============
        // ===================================

        #region Delete Methods

        /// <summary>
        /// Soft delete a user by Id.
        /// </summary>
        [HttpDelete( "{id:guid}" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> Delete( Guid id )
        {
            // Send the delete command to the handler
            var result = await _mediator.Send( new DeleteUserCommand { UserId = id } );

            // If not found, return 404
            if( !result )
                return NotFound( );

            // Otherwise, return success message
            return Ok( new { message = "User deleted successfully" } );
        }

        #endregion
    }
}
