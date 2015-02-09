using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharedDaemonLib
{
    [Serializable]
    public class SystemHellModules
    {
       public SystemHellModules()
        {
            Modules = new List<Daemon>();
        }
        public List<Daemon> Modules { get; set; }
    }
}