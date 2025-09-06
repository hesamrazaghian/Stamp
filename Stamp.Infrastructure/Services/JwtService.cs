using Microsoft.Extensions.Configuration; 
using Microsoft.IdentityModel.Tokens;    
using Stamp.Application.Interfaces;      
using Stamp.Domain.Enums;                
using System.IdentityModel.Tokens.Jwt;    
using System.Security.Claims;             
using System.Text;                        

namespace Stamp.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration; // Holds configuration data from appsettings.json

        public JwtService( IConfiguration configuration )
        {
            _configuration = configuration; // Inject configuration into the service
        }

        // Generate token using default lifetime from appsettings.json
        public string GenerateToken( Guid userId, RoleEnum role, string? email )
        {
            // Read token lifetime from configuration
            var lifetimeConfig = _configuration[ "JwtSettings:TokenLifetimeMinutes" ];

            // Parse lifetimeConfig to integer, default to 60 if invalid
            if( !int.TryParse( lifetimeConfig, out var expireMinutes ) )
                expireMinutes = 60;

            // Call the overloaded method that accepts a custom lifetime
            return GenerateToken( userId, role, email, expireMinutes );
        }

        // Generate token with a custom expiration time
        public string GenerateToken( Guid userId, RoleEnum role, string? email, int expireMinutes )
        {
            // Read secret key from configuration
            var secret = _configuration[ "JwtSettings:Secret" ];
            if( string.IsNullOrEmpty( secret ) )
                throw new Exception( "JWT Secret is not configured." );

            // Create claims (user-related information stored in the token)
            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString()),         // Custom claim for user ID
                new Claim(ClaimTypes.Role, role.ToString())     // User role claim
            };

            // Optionally add email claim if provided
            if( !string.IsNullOrWhiteSpace( email ) )
                claims.Add( new Claim( ClaimTypes.Email, email ) );

            // Create a symmetric security key using the secret
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( secret ) );

            // Create signing credentials using the key and HMAC SHA256 algorithm
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

            // Create the JWT token with claims and expiration date
            var token = new JwtSecurityToken(
                claims: claims,                                    // Token payload
                expires: DateTime.UtcNow.AddMinutes( expireMinutes ), // Expiration date
                signingCredentials: creds                           // Token signature
            );

            // Return the serialized JWT token string
            return new JwtSecurityTokenHandler( ).WriteToken( token );
        }
    }
}
