using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying
{
    public static class FilterOperations
    {
        public const string Eq = "eq";
        public const string Contains = "contains";
        public const string StartsWith = "startsWith";
        public const string EndsWith = "endsWith";

        public const string Gt = "gt";
        public const string Gte = "gte";
        public const string Lt = "lt";
        public const string Lte = "lte";
        public const string Between = "between";

        public const string Any = "any";   // za MultiSelect
        public const string All = "all";   // za MultiSelect
        public const string None = "none"; // za MultiSelect
    }
}
