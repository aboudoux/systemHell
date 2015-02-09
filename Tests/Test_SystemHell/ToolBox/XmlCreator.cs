using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test_SystemHell.ToolBox
{
    public class XmlCreator : IDisposable
    {
        private readonly string _xmlFile;
        public string XmlFilePath { get { return _xmlFile; } }

        public XmlCreator()
        {
            _xmlFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Guid.NewGuid() + ".xml");
            File.WriteAllText(_xmlFile, "");
        }

        public void Dispose()
        {
            try {
                File.Delete(_xmlFile);
            }
            catch {                
            }
        }
    }
}
