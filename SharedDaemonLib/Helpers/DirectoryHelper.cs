using System.IO;
using System.Reflection;

namespace SharedDaemonLib.Helpers
{
    public static class DirectoryHelper
    {
        private static readonly string _currentDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        public static string CurrentDirectory { get { return _currentDirectory; } }

        private const string ModuleConfigfileName = "Daemons.xml";
        private static readonly string _daemonFilePath = Path.Combine(_currentDirectory, ModuleConfigfileName);
        public static string DaemonFilePath { get { return _daemonFilePath; } }
    }
}
