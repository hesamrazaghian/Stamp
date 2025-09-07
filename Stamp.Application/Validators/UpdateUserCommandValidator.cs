using FluentValidation;
using Stamp.Application.Commands.Users;

namespace Stamp.Application.Validators
{
    /// <summary>
    /// FluentValidation rules for validating user update request.
    /// </summary>
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator( )
        {
            RuleFor( x => x.UserId )
                .NotEmpty( ).WithMessage( "UserId is required." );

            RuleFor( x => x.Email )
                .MaximumLength( 256 ).WithMessage( "Email cannot exceed 256 characters." )
                .EmailAddress( ).When( x => !string.IsNullOrWhiteSpace( x.Email ) )
                .WithMessage( "Invalid email address format." );

            RuleFor( x => x.Phone )
                .MaximumLength( 20 ).WithMessage( "Phone number cannot exceed 20 characters." );

            RuleFor( x => x.Role )
                .IsInEnum( ).WithMessage( "Invalid role specified." );
        }
    }
}
