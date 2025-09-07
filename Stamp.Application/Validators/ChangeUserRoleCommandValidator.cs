using FluentValidation;
using Stamp.Application.Commands.Users;

namespace Stamp.Application.Validators
{
    /// <summary>
    /// FluentValidation rules for validating change user role request.
    /// </summary>
    public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleCommandValidator( )
        {
            RuleFor( x => x.UserId )
                .NotEmpty( ).WithMessage( "UserId is required." );

            RuleFor( x => x.NewRole )
                .IsInEnum( ).WithMessage( "Invalid role specified." );
        }
    }
}
