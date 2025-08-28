using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Stamp.Application.Interfaces;
using Stamp.Application.Settings;

namespace Stamp.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService( JwtSettings jwtSettings )
    {
        _jwtSettings = jwtSettings;
    }

    public string GenerateToken( Guid userId, Guid tenantId, string role, string email )
    {
        var claims = new[ ]
        {
        new Claim("UserId", userId.ToString()),             // مهم برای /me
        new Claim("TenantId", tenantId.ToString()),
        new Claim(ClaimTypes.Role, role),
        new Claim(ClaimTypes.Email, email),
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _jwtSettings.Secret ) );
        var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes( _jwtSettings.TokenLifetimeMinutes ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler( ).WriteToken( token );
    }


}