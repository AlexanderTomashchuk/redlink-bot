using System;

namespace Domain.Common
{
    public class AuditableEntity
    {
        public DateTime CreatedOn { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long ModifiedBy { get; set; }
    }
}