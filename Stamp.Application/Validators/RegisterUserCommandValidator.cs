using FluentValidation;
using Stamp.Application.Commands.Users;

namespace Stamp.Application.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator( )
    {
        RuleFor( x => x.Email )
            .NotEmpty( ).WithMessage( "ایمیل الزامی است" )
            .EmailAddress( ).WithMessage( "ایمیل معتبر نیست" )
            .MaximumLength( 256 ).WithMessage( "ایمیل نمی‌تواند بیش از 256 کاراکتر باشد" );

        RuleFor( x => x.Password )
            .NotEmpty( ).WithMessage( "رمز عبور الزامی است" )
            .MinimumLength( 8 ).WithMessage( "رمز عبور باید حداقل 8 کاراکتر باشد" );

        RuleFor( x => x.TenantId )
            .NotEmpty( ).WithMessage( "شناسه سازمان الزامی است" );
    }
}