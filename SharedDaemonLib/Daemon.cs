using System;
using System.Xml.Serialization;

namespace SharedDaemonLib
{
    [Serializable]
    public class Daemon
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public bool Actif { get; set; }
        [XmlAttribute]
        public string Assembly { get; set; }
        [XmlAttribute]
        public string ModuleType { get; set; }

        public object Configuration { get; set; }
    }
}