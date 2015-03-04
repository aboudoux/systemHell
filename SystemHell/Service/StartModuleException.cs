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
        public StartModuleException(List<Exception> inner) :
            base("Errors occurred when starting some modules")            
        {
            inner.ForEach(a=>Trace.WriteLine(a));
        }                 
    }
}
