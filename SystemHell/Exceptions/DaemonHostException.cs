using System;
using System.Runtime.Serialization;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class DaemonHostException : Exception
    {      
        public DaemonHostException()
        {
        }

        public DaemonHostException(string message) : base(message)
        {
        }

        public DaemonHostException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DaemonHostException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}