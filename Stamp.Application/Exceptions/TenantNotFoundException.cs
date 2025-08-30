using System;

namespace Stamp.Application.Exceptions;

public class TenantNotFoundException : Exception
{
    public TenantNotFoundException( Guid tenantId )
        : base( $"Tenant with ID {tenantId} not found" ) { }
}