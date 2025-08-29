using MediatR;
using Stamp.Application.DTOs;

namespace Stamp.Application.Queries.Tenants;

public class GetAllTenantsQuery : IRequest<List<TenantDto>>
{
}