using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Entities
{
    public interface IEntity
    {
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long? DeletedBy { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public void CreatedByUser(long userId);
        public void UpdatedByUser(long userId);
        public void DeletedByUser(long userId);
    }
}
