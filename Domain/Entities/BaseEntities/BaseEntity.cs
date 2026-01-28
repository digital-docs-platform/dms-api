using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.BaseEntities
{
    public abstract class BaseEntity<TKey>
       where TKey : notnull
    {
        public TKey Id { get; set; } = default!;
    }

}
