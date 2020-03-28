using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.System
{
    public class StringEqualityComparer : EqualityComparer<string>
    {
        public override bool Equals(string x, string y)
        {
            Contract.Requires(x != null && y != null);
            return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode(string value)
        {
            Contract.Requires(value != null);
            return value.ToUpperInvariant().GetHashCode();
        }
    }
}
