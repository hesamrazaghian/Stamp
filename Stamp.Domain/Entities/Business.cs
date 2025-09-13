using System;

namespace Stamp.Domain.Entities
{
    /// <summary>Represents a business in the multi-business loyalty system.</summary>
    public class Business : BaseEntity
    {
        #region Properties
        /// <summary>Name of the business.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>The unique identifier of the user who owns this business.</summary>
        public Guid OwnerUserId { get; set; }
        /// <summary>Status of the business (Active/Inactive).</summary>
        public bool IsActive { get; set; } = true;
        #endregion
    }
}
