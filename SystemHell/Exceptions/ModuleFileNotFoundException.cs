using System;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class ModuleFileNotFoundException : ModuleLoaderException
    {        
        public ModuleFileNotFoundException(string filePath) : base("the '" + filePath + "' file to load modules is not found.")
        {
        }     
    }
}