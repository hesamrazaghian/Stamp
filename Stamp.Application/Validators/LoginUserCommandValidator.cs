using FluentValidation;
using Stamp.Application.Commands.Users;

namespace Stamp.Application.Validators
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator( )
        {
            RuleFor( x => x.Email )
                .NotEmpty( ).WithMessage( "Email is required" )
                .EmailAddress( ).WithMessage( "Invalid email format" );

            RuleFor( x => x.Password )
                .NotEmpty( ).WithMessage( "Password is required" );
        }
    }
}
