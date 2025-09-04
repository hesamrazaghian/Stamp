using Stamp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stamp.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public async Task<string> HashPasswordAsync( string password )
        {
            return await Task.Run( ( ) =>
            {
                return BCrypt.Net.BCrypt.HashPassword( BCrypt.Net.BCrypt.GenerateSalt( ), password );
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
}
