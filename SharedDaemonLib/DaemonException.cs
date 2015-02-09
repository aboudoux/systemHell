using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace SharedDaemonLib
{
    [Serializable]
    public class DaemonException : Exception
    {       
        public DaemonException()
        {
            WriteTrace();
        }

        public DaemonException(string message) : base(message)
        {
            WriteTrace();
        }

        public DaemonException(string message, Exception inner) : base(message, inner)
        {
            WriteTrace();
        }

        protected DaemonException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
        
        private void WriteTrace()
        {
            Trace.WriteLine(ToString());
        }
    }
}