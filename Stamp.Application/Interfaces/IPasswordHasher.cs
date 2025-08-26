using System.Threading.Tasks;

namespace Stamp.Application.Interfaces;

public interface IPasswordHasher
{
    Task<string> HashPasswordAsync( string password );
    Task<bool> VerifyPasswordAsync( string hashedPassword, string providedPassword );
}