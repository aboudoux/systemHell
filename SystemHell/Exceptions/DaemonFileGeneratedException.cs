using System;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class DaemonFileGeneratedException : ModuleLoaderException
    {
        public DaemonFileGeneratedException(string xmlFilePath)
            : base("The service has stopped because no configuration file was present. a file has been generated in '" + xmlFilePath + "'. Please configure and restart the service.")
        {
        }      
    }
}