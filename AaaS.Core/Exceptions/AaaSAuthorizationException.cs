using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Exceptions
{
    public class AaaSAuthorizationException : Exception
    {
        public AaaSAuthorizationException()
        {
        }

        public AaaSAuthorizationException(string message) : base(message)
        {
        }

        public AaaSAuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AaaSAuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
