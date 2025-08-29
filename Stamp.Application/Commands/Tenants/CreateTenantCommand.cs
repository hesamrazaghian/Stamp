using MediatR;
using Stamp.Application.DTOs;

namespace Stamp.Application.Commands.Tenants;

public class CreateTenantCommand : IRequest<TenantDto>
{
    public string Name { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
}