using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.Business
{
    [Serializable]
    public class NzbException : Exception
    {
        public NzbException()
        {
        }

        public NzbException(string message) : base(message)
        {
        }

        public NzbException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NzbException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
    }
}
