using FluentValidation;
using Stamp.Application.Commands.Users;

namespace Stamp.Application.Validators
{
    /// <summary>
    /// اعتبارسنجی ورود کاربر
    /// </summary>
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator( )
        {
            RuleFor( x => x.Email )
                .NotEmpty( ).WithMessage( "ایمیل الزامی است" )
                .EmailAddress( ).WithMessage( "فرمت ایمیل معتبر نیست" );

            RuleFor( x => x.Password )
                .NotEmpty( ).WithMessage( "رمز عبور الزامی است" );

            RuleFor( x => x.TenantId )
                .NotEmpty( ).WithMessage( "شناسه سازمان الزامی است" );
        }
    }
}
