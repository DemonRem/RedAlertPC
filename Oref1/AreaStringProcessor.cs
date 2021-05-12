using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public static class AreaStringProcessor
    {
        private static readonly char[] _comma = new char[] { ',' };

        public static IEnumerable<string> ProcessAreaStrings(this IEnumerable<string> areaStrings)
        {
            return areaStrings.SelectMany(multipleIds => multipleIds.Split(_comma, StringSplitOptions.RemoveEmptyEntries))
                              .ProcessAreaStringsInternal();
        }

        public static IEnumerable<string> ProcessAreaString(this string areaString)
        {
            return areaString.Split(_comma, StringSplitOptions.RemoveEmptyEntries)
                             .ProcessAreaStringsInternal();
        }

        private static IEnumerable<string> ProcessAreaStringsInternal(this IEnumerable<string> areaStrings)
        {
            return areaStrings.Select(area => area.Trim())
                              .Select(area => GetWithoutAreaWord(area))
                              .Distinct();
        }

        private static string GetWithoutAreaWord(string areaString)
        {
            if (areaString.StartsWith("מרחב "))
            {
                return areaString.Substring(5).Trim();
            }
            else
            {
                return areaString;
            }
        }
    }
}
