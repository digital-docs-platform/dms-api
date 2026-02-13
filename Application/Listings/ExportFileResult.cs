using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Listings
{
    public sealed class ExportFileResult
    {
        public required byte[] Content { get; init; }
        public required string ContentType { get; init; }
        public required string FileName { get; init; }
    }
}
