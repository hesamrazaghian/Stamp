using Stamp.Application.DTOs;
using Stamp.Application.Commands.Users;
using System.Threading;
using System.Threading.Tasks;

namespace Stamp.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync( RegisterUserCommand command, CancellationToken cancellationToken );
}