using BCrypt.Net;
using Stamp.Application.Interfaces;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public async Task<string> HashPasswordAsync( string password )
    {
        // چون BCrypt خودش عملیات سنگین رو مدیریت می‌کنه، ولی ما async می‌خوایم
        // از Task.Run استفاده می‌کنیم تا در Thread جدا اجرا بشه
        return await Task.Run( ( ) =>
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt( );
            return BCrypt.Net.BCrypt.HashPassword( password, salt );
        } );
    }

    public async Task<bool> VerifyPasswordAsync( string hashedPassword, string providedPassword )
    {
        return await Task.Run( ( ) =>
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify( providedPassword, hashedPassword );
            }
            catch
            {
                return false;
            }
        } );
    }
}