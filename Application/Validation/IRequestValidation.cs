using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public interface IRequestValidation
    {
        public Task ValidateAsync<T>(T request, CancellationToken ct);
    }
}
