using System;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class ModuleFileEmptyException : ModuleLoaderException
    {
        public ModuleFileEmptyException(string filePath)
            : base("the '" + filePath + "' file to load modules is empty.")
        {
        }
    }
}