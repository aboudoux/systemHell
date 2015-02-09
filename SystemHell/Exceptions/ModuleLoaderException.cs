using System;
using System.Runtime.Serialization;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class ModuleLoaderException : Exception
    {     
        public ModuleLoaderException()
        {
        }

        public ModuleLoaderException(string message) : base(message)
        {
        }

        public ModuleLoaderException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ModuleLoaderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
