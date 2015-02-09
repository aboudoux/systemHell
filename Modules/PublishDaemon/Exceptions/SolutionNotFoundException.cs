using System;
using System.Runtime.Serialization;
using SharedDaemonLib;

namespace PublishDaemon.Exceptions
{
    [Serializable]
    public class SolutionNotFoundException : DaemonException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SolutionNotFoundException()
        {
        }

        public SolutionNotFoundException(string message)
            : base(message)
        {
        }

        public SolutionNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SolutionNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}