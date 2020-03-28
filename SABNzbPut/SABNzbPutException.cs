using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SABPut
{
    [Serializable]
    public class SABNzbPutException : ArgumentException
    {

        public SABNzbPutException(string message)
                : base(message)
        {

        }

        protected SABNzbPutException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public SABNzbPutException()
        {
        }

        public SABNzbPutException(string message, Exception innerException) : base(message, innerException)
        {
        }

   }
}
