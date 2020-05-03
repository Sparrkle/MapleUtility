using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Helpers
{
    public static class RegexHelper
    {
        private static readonly Regex rxNonDigits = new Regex(@"[^\d]+");

        // simply replace the offending substrings with an empty string
        public static string CleanStringOfNonDigits(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            string cleaned = rxNonDigits.Replace(s, "");
            return cleaned;
        }
    }
}
