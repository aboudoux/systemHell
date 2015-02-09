using System.Diagnostics;

namespace SharedDaemonLib
{
    public static class Tracer
    {
        public static void Write(string moduleName, string format, params string[] args)
        {
            Trace.WriteLine("[SystemHell][" + moduleName + "] - " + string.Format(format, args));
        }
    }
}