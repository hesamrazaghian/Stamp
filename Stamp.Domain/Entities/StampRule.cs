namespace Stamp.Domain.Entities
{
    public class StampRule : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int PurchaseAmountThreshold { get; set; } // مثلا هر 10000 تومان
        public int StampsAwarded { get; set; }            // مثلا 1 مهر
        public bool IsActive { get; set; } = true;

        // Navigation
        public Tenant Tenant { get; set; } = null!;
    }
}
