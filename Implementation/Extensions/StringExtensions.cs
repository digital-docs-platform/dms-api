using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Extensions
{
    public static partial class StringExtensions
    {
        public static string Slugify(this string input, string fallback = "field")
        {
            input = input.Trim().ToLowerInvariant();
            var sb = new StringBuilder(input.Length);

            bool underscore = false;
            foreach (var ch in input)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    sb.Append(ch);
                    underscore = false;
                }
                else
                {
                    if (!underscore)
                    {
                        sb.Append('_');
                        underscore = true;
                    }
                }
            }

            var result = sb.ToString().Trim('_');
            return string.IsNullOrWhiteSpace(result) ? "field" : result;
        }
    }
}
