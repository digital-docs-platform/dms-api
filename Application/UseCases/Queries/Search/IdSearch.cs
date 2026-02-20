using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Search
{
    public sealed class IdSearch<TType>
    {
        public TType Id { get; set; }
    }
}
