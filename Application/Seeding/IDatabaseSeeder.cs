using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Seeding
{
    public interface IDatabaseSeeder
    {
        public Task SeedAsync(CancellationToken ct = default);
    }
}
