using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Nzb.System
{
    [Serializable]
    public class NzbContextDictionary : Dictionary<string, object>
    {
        public const string PrincipaleStopwath = "Principale.Stopwath";
        private class StringEqualityComparer : EqualityComparer<string>
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
        public static NzbContextDictionary Current { get; private set; }
        private NzbContextDictionary() : base(new StringEqualityComparer()) { }

        protected NzbContextDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static NzbContextDictionary Create()
        {
            return NzbContextDictionary.Current = new NzbContextDictionary();
        }

        public T Get<T>(string key)
        {
            var retour = !this.TryGetValue(key, out object val1)
                         ? default
                         : (T)val1;
            return retour;
        }
    }
}
