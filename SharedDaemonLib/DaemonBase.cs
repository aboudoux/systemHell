using System;
using System.Threading;

namespace SharedDaemonLib
{
    public abstract class DaemonBase<T> : IDaemonModule
        where T : class , IDaemonModuleConfiguration, new() 
    {
        private string _moduleName = string.Empty;

        public string ModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; }
        }

        public abstract void Start(CancellationToken cancellationToken);
        
        public Type ConfigurationType { get { return typeof (T); } }
        private object _config;
        object IDaemonModule.Configuration
        {
            get
            {
                if (_config != null)
                    return _config;
                return _config ?? (_config = new T());                                
            }
            set
            {
                if( value != null && !value.GetType().IsAssignableFrom(typeof(T)))
                    throw new Exception("Vous ne pouvez pas défini un objet de configuration de type " + value.GetType() + " car le démon attend une configuration de type " + typeof(T));
                _config = value;
            }
        }        
        protected T Configuration
        {
            get { return (this as IDaemonModule).Configuration as T; }
        }

        protected void WriteTrace(string format, params string[] args)
        {
            Tracer.Write(ModuleName, format, args);
        }
    }
}