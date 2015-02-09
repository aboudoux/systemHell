using System;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class DaemonFileGeneratedException : ModuleLoaderException
    {     
        public DaemonFileGeneratedException(string xmlFilePath) : base("Le service s'est arrêté car aucun fichier de configuration n'était présent. un fichier a été généré dans '" + xmlFilePath +"'. Veuillez le configurer et relancer le service.")
        {
        }      
    }
}