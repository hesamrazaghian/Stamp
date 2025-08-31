using MediatR;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Application.Queries.Users;
using Stamp.Domain.Enums;

namespace Stamp.Application.Handlers.Users;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler( IUserRepository userRepository )
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> Handle( GetCurrentUserQuery request, CancellationToken cancellationToken )
    {
        var user = await _userRepository.GetWithTenantsAsync( request.UserId, cancellationToken );

        if( user == null )
            throw new Exception( "کاربر یافت نشد" );

        // 🎯 نقش را از DB بخوان و اگر معتبر نبود → Guest
        if( !Enum.TryParse<RoleEnum>( user.Role, true, out var roleEnum ) )
        {
            roleEnum = RoleEnum.Guest;
        }

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,

            // ✅ ارسال مقدار RoleEnum به جای رشته خالی
            Role = roleEnum.ToString( ),

            CreatedAt = user.CreatedAt,
            Tenants = user.UserTenants
                .Where( ut => !ut.IsDeleted )
                .Select( ut => new TenantMembershipDto
                {
                    TenantId = ut.TenantId,
                    TenantName = ut.Tenant.Name
                } ).ToList( )
        };
    }
}
