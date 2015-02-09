using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WorkitemDaemon.Helpers
{
    public static class XmlHelper
    {
        public static void LoadFromXMLFile<T>(string XMLFilePath, ref T SerializableObject)
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            TextReader r = new StreamReader(XMLFilePath);
            XmlReader xmlReader = XmlReader.Create(r, new XmlReaderSettings() { CheckCharacters = true });
            SerializableObject = (T)s.Deserialize(xmlReader);
            r.Close();
            xmlReader.Close();
        }

        public static void SaveToXMLFile<T>(string XMLFilePath, T SerializableObject)
        {
            XmlSerializer serialiseur = new XmlSerializer(typeof(T));
            XmlWriter xmlWriter = XmlWriter.Create(XMLFilePath, new XmlWriterSettings() { CheckCharacters = true, Indent = true });
            serialiseur.Serialize(xmlWriter, SerializableObject);
            xmlWriter.Close();
        }
    }
}
