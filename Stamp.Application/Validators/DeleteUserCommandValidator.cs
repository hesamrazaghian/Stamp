using FluentValidation;
using Stamp.Application.Commands.Users;

namespace Stamp.Application.Validators
{
    /// <summary>
    /// FluentValidation rules for validating delete user request.
    /// </summary>
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator( )
        {
            RuleFor( x => x.UserId )
                .NotEmpty( )
                .WithMessage( "UserId is required." );

            // SoftDelete is boolean, no extra rule needed at the moment.
        }
    }
}
