using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.BaseEntities
{
    public abstract class ActivatableEntity : AuditableEntity
    {
        public bool IsActive { get; set; } = true;
    }
}
