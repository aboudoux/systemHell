using System;
using System.Runtime.Serialization;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class ModuleFileNotFoundException : ModuleLoaderException
    {        
        public ModuleFileNotFoundException(string filePath) : base("Le fichier '" + filePath + "' permettant de charger les modules est introuvable.")
        {
        }     
    }
}