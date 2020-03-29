using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.System.Types
{
    public static class StringExtension
    {
        public static string TrimFullToEnd(this string originalValue)
        {
            Contract.Requires(originalValue != null);
            var suppressChars = new char[] { ' ', '\b', '\r', '\n' };
            var retour = originalValue.TrimEnd(suppressChars);
            return retour;
        }
    }
}
