using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.Business
{
    [Serializable]
    public class NzbZipException : Exception
    {
        public NzbZipException()
        {
        }

        public NzbZipException(string message) : base(message)
        {
        }

        public NzbZipException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NzbZipException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
    }
}
