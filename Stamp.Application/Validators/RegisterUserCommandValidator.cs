using FluentValidation;
using Stamp.Application.Commands.Users;

namespace Stamp.Application.Validators
{
    /// <summary>
    /// FluentValidation rules for validating user registration request.
    /// </summary>
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator( )
        {
            RuleFor( x => x.Email )
                .NotEmpty( ).WithMessage( "Email is required" )
                .EmailAddress( ).WithMessage( "Invalid email address format" )
                .MaximumLength( 256 ).WithMessage( "Email cannot exceed 256 characters" );

            RuleFor( x => x.Phone )
                .NotEmpty( ).WithMessage( "Phone number is required" )
                .MaximumLength( 20 ).WithMessage( "Phone number cannot exceed 20 characters" );

            RuleFor( x => x.Password )
                .NotEmpty( ).WithMessage( "Password is required" )
                .MinimumLength( 8 ).WithMessage( "Password must be at least 8 characters long" );
        }
    }
}
