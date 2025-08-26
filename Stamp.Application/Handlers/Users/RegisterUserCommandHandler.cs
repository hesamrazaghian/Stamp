using MediatR;
using Stamp.Application.DTOs;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Handlers.Users;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle( RegisterUserCommand command, CancellationToken cancellationToken )
    {
        // 1. چک تکراری بودن ایمیل (در سطح Tenant)
        if( await _userRepository.ExistsByEmailAsync( command.Email, command.TenantId, cancellationToken ) )
        {
            throw new Exception( "ایمیل قبلاً ثبت شده است" );
        }

        // 2. هش کردن رمز عبور
        var passwordHash = await _passwordHasher.HashPasswordAsync( command.Password );

        // 3. ساخت کاربر جدید
        var user = new User
        {
            Id = Guid.NewGuid( ),
            Email = command.Email,
            Phone = command.Phone,
            PasswordHash = passwordHash,
            Role = command.Role,
            TenantId = command.TenantId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        // 4. ذخیره کاربر در دیتابیس
        await _userRepository.AddAsync( user, cancellationToken );

        // 5. تبدیل به DTO و برگرداندن
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            TenantId = user.TenantId,
            CreatedAt = user.CreatedAt
        };
    }
}