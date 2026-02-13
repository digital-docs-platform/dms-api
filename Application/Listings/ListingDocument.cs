using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Listings
{
    public sealed class ListingDocument
    {
        public string Title { get; init; } = "Listing";
        public required List<string> Headers { get; init; }
        public required List<List<string>> Rows { get; init; }
    }
}
