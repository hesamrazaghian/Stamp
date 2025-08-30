using MediatR;
using Stamp.Application.Commands.UserTenants;
using Stamp.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Handlers.UserTenants;

public class JoinTenantCommandHandler : IRequestHandler<JoinTenantCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;

    public JoinTenantCommandHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository )
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
    }

    public async Task Handle(
        JoinTenantCommand command,
        CancellationToken cancellationToken )
    {
        // ✅ خط ۱: بررسی وجود Tenant
        var tenant = await _tenantRepository.GetByIdAsync(
            command.TenantId,
            cancellationToken );

        if( tenant == null )
            throw new Exception( "Tenant not found" );

        // ✅ خط ۲: بررسی عضویت قبلی کاربر
        var isMember = await _userRepository.ExistsInTenantAsync(
            command.UserId,
            command.TenantId,
            cancellationToken );

        if( isMember )
            throw new Exception( "User is already a member of this tenant" );

        // ✅ خط ۳: افزودن کاربر به Tenant
        await _userRepository.AddToTenantAsync(
            command.UserId,
            command.TenantId,
            cancellationToken );
    }
}