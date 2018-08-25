using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant.Additions
{
    public static class AdditionsString
    {
        public static string RemoveStartText(this string text, string find)
        {
            if (text.StartsWith(find))
                return text.Substring(find.Length);
            else
                return text;
        }

        public static string RemoveEndText(this string text, string find)
        {
            if (text.EndsWith(find))
                return text.Remove(text.Length - find.Length);
            else
                return text;
        }
    }
}
