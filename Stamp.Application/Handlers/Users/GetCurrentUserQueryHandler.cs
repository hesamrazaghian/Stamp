using MediatR;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Application.Queries.Users;

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

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
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
