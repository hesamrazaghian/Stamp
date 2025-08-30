using MediatR;
using Stamp.Application.DTOs;
using Stamp.Application.Commands.Users;
using Stamp.Application.Interfaces;
using Stamp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Handlers.Users
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            ITenantRepository tenantRepository,
            IPasswordHasher passwordHasher )
        {
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle( RegisterUserCommand command, CancellationToken cancellationToken )
        {
            // 1. جستجوی کاربر با ایمیل (global)
            var existingUser = await _userRepository.GetByEmailAsync( command.Email, cancellationToken );

            if( existingUser != null )
            {
                if( command.TenantId.HasValue )
                {
                    // 2. آیا کاربر عضو این Tenant شده؟
                    var isMember = await _userRepository.ExistsInTenantAsync(
                        existingUser.Id,
                        command.TenantId.Value,
                        cancellationToken
                    );

                    if( isMember )
                    {
                        throw new Exception( "ایمیل قبلاً در این Tenant ثبت شده است" );
                    }

                    // ✅ بررسی اولین عضویت کاربر (برای تغییر نقش)
                    var hasOtherMemberships = await _userRepository.HasAnyTenantMembershipAsync(
                        existingUser.Id,
                        cancellationToken
                    );

                    // 3. افزودن به Tenant جدید
                    await _userRepository.AddToTenantAsync(
                        existingUser.Id,
                        command.TenantId.Value,
                        cancellationToken
                    );

                    // ✅ تغییر نقش از Guest به User در اولین عضویت
                    if( !hasOtherMemberships )
                    {
                        existingUser.Role = "User";
                        await _userRepository.UpdateUserRoleAsync(
                            existingUser.Id,
                            "User",
                            cancellationToken
                        );
                    }
                }

                return new UserDto
                {
                    Id = existingUser.Id,
                    Email = existingUser.Email,
                    Phone = existingUser.Phone,
                    Role = existingUser.Role,
                    CreatedAt = existingUser.CreatedAt,
                    Tenants = existingUser.UserTenants
                        .Where( ut => !ut.IsDeleted )
                        .Select( ut => new UserTenantDto
                        {
                            TenantId = ut.TenantId,
                            TenantName = ut.Tenant.Name,
                            UserId = ut.UserId,
                            Email = ut.User.Email
                        } ).ToList( )
                };
            }

            // 4. ایجاد کاربر جدید
            var passwordHash = await _passwordHasher.HashPasswordAsync( command.Password );

            var newUser = new User
            {
                Id = Guid.NewGuid( ),
                Email = command.Email,
                Phone = command.Phone,
                PasswordHash = passwordHash,
                Role = "User",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            // اگر TenantId داده شده → ایجاد با Tenant
            if( command.TenantId.HasValue )
            {
                var tenant = await _tenantRepository.GetByIdAsync(
                    command.TenantId.Value,
                    cancellationToken
                );

                if( tenant == null )
                    throw new Exception( "Tenant not found" );

                await _userRepository.CreateWithTenantAsync(
                    newUser,
                    command.TenantId.Value,
                    cancellationToken
                );
            }
            else
            {
                // بدون Tenant (کاربر مهمان)
                await _userRepository.AddAsync( newUser, cancellationToken );
            }

            return new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Phone = newUser.Phone,
                Role = newUser.Role,
                CreatedAt = newUser.CreatedAt,
                Tenants = command.TenantId.HasValue
                    ? new List<UserTenantDto>
                      {
                          new UserTenantDto
                          {
                              TenantId = command.TenantId.Value,
                              TenantName = (await _tenantRepository.GetByIdAsync(command.TenantId.Value, cancellationToken))?.Name ?? string.Empty,
                              UserId = newUser.Id,
                              Email = newUser.Email
                          }
                      }
                    : new List<UserTenantDto>( )
            };
        }
    }
}