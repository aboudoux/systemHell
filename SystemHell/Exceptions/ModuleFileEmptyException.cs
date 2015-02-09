using System;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class ModuleFileEmptyException : ModuleLoaderException
    {
        public ModuleFileEmptyException(string filePath)
            : base("Le fichier '" + filePath + "' permettant de charger les modules est vide.")
        {
        }
    }
}