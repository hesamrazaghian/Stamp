using System;

namespace Stamp.Domain.Entities
{
    // Represents a customer within a specific business loyalty program
    public class Customer : BaseEntity
    {
        #region Properties
        // Id of the business this customer belongs to (Tenant filtering)
        public Guid BusinessId { get; set; }
        // Full name of the customer
        public string FullName { get; set; } = string.Empty;
        // Phone number of the customer
        public string Phone { get; set; } = string.Empty;
        // Email address of the customer (optional)
        public string? Email { get; set; }
        // Customer's date of birth for birthday-based rewards
        public DateTime? DateOfBirth { get; set; }
        // Current total points accumulated by the customer
        public int TotalPoints { get; set; }
        // Indicates whether the customer is currently active
        public bool IsActive { get; set; } = true;
        #endregion
    }
}
