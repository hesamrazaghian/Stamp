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

    public string GenerateToken( Guid userId, Guid tenantId, string role )
    {
        // 1. ساخت Claimها
        var claims = new[ ]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("TenantId", tenantId.ToString()), // برای Multi-Tenant
            new Claim(ClaimTypes.Role, role),          // برای دسترسی‌ها
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // جلوگیری از Replay Attack
        };

        // 2. ساخت کلید امنیتی
        var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _jwtSettings.Secret ) );
        var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

        // 3. ساخت توکن
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes( _jwtSettings.TokenLifetimeMinutes ),
            signingCredentials: creds
        );

        // 4. برگرداندن توکن به صورت رشته
        return new JwtSecurityTokenHandler( ).WriteToken( token );
    }
}