using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using SharedDaemonLib;

namespace SystemHell.Service
{
    [Serializable]
    public class StartModuleException : DaemonException
    {
        public StartModuleException()
        {            
        }

        public StartModuleException(string message) : base(message)
        {         
        }

        public StartModuleException(string message, Exception inner) : base(message, inner)
        {         
        }

        public StartModuleException(string message, List<Exception> inner) : base(message)            
        {
            inner.ForEach(a=>Trace.WriteLine(a));
        }

        protected StartModuleException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }              
    }
}
