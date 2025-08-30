namespace Stamp.Application.DTOs;

public class TenantListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
}