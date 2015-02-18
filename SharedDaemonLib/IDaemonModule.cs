using System;
using System.Threading;
using System.Xml.Serialization;

namespace SharedDaemonLib
{
    public interface IDaemonModule
    {
        string ModuleName { get; set; }
        void Start(CancellationToken cancellationToken);
        Type ConfigurationType { get; }
        object Configuration { get; set; }
        bool DaemonStarted { get; }
    }

    public interface IDaemonModuleConfiguration
    {
        string TypeFullName { get; set; }
    }

    public abstract class DaemonModuleConfiguration : IDaemonModuleConfiguration
    {
        [XmlAttribute]
        public string TypeFullName { get { return GetType().FullName; } set{ } }
    }
}
