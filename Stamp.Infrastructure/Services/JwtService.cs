using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stamp.Application.DTOs;
using Stamp.Application.Interfaces;
using Stamp.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Stamp.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        // Holds the strongly typed JWT settings from configuration
        private readonly JwtSettings _jwtSettings;

        // Constructor - inject JwtSettings via IOptions
        public JwtService( IOptions<JwtSettings> jwtSettings )
        {
            _jwtSettings = jwtSettings.Value;
        }

        // Generate token using default expiration from configuration
        public string GenerateToken( Guid userId, RoleEnum role, string? email )
        {
            // Prepare claims list for JWT payload
            var claims = new List<Claim>
            {
                // Custom claim for UserId
                new Claim("UserId", userId.ToString()),
                // Built-in role claim
                new Claim(ClaimTypes.Role, role.ToString())
            };

            // Optionally add email claim if it is provided
            if( !string.IsNullOrWhiteSpace( email ) )
                claims.Add( new Claim( ClaimTypes.Email, email ) );

            // Create symmetric security key from SecretKey
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _jwtSettings.SecretKey ) );

            // Signing credentials using HMAC SHA256
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes( _jwtSettings.ExpiresInMinutes ),
                signingCredentials: creds
            );

            // Serialize and return the token
            return new JwtSecurityTokenHandler( ).WriteToken( token );
        }

        // Generate token with custom expiration
        public string GenerateToken( Guid userId, RoleEnum role, string? email, int expireMinutes )
        {
            var originalExpiry = _jwtSettings.ExpiresInMinutes;
            _jwtSettings.ExpiresInMinutes = expireMinutes;
            var token = GenerateToken( userId, role, email );
            _jwtSettings.ExpiresInMinutes = originalExpiry; // Restore original value
            return token;
        }
    }
}
