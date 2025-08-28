using System;

namespace Stamp.Application.Interfaces
{
    public interface ICurrentTenantService
    {
        Guid TenantId { get; }
    }
}
