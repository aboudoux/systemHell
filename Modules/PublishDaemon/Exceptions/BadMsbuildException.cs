using System;
using System.Runtime.Serialization;

namespace PublishDaemon.Exceptions
{
    [Serializable]
    public class BadMsbuildException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BadMsbuildException()
        {
        }

        public BadMsbuildException(string message) : base(message)
        {
        }

        public BadMsbuildException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BadMsbuildException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}