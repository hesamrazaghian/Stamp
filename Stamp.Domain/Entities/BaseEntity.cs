using System;

namespace Stamp.Domain.Entities;

/// <summary>
/// موجودیت پایه که همه موجودیت‌های سیستم از آن ارث‌بری می‌کنند.
/// شامل اطلاعات عمومی مثل TenantId، Soft Delete، تاریخ ایجاد و شناسه.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid( );  // کلید اصلی یکتا
    public Guid TenantId { get; set; }              // برای Multi-Tenant
    public bool IsDeleted { get; set; } = false;    // Soft Delete
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // زمان ایجاد
}
