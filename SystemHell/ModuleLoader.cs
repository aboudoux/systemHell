using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using SystemHell.Exceptions;
using SharedDaemonLib;

namespace SystemHell
{
    public static class ModuleLoader
    {
        public static List<IDaemonModule> GetAllDaemon(string xmlFilePath)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath))
                throw new ModuleFileNotFoundException(xmlFilePath);

            var result = new List<IDaemonModule>();
            
            var modules = LoadModuleXmlFile(xmlFilePath);
            foreach (var daemonModule in modules.Modules.Where(a=>a.Actif)) {
                var assembly = Assembly.Load(daemonModule.Assembly);
                var daemonType = assembly.GetType(daemonModule.ModuleType);
                if( daemonType == null )
                    throw new Exception("Impossible de trouver le type " + daemonModule.ModuleType + "dans l'assembly " + daemonModule.Assembly + " tel que défini dans le fichier de configuration ");
                if( daemonType.GetInterface(typeof(IDaemonModule).Name) == null )
                    throw new Exception("Le module " + daemonModule.Name + " n'hérite pas de l'interface " + typeof(IDaemonModule).Name );
                
                var instance = Activator.CreateInstance(daemonType) as IDaemonModule;
                if( instance == null )
                    throw new Exception("Impossible de créer une instance du module " + daemonModule.Name);

                instance.ModuleName = daemonModule.Name;
                instance.Configuration = DeserializeConfiguration(daemonModule.Configuration as XmlNode[], assembly);
                result.Add(instance);
            }
            return result;
        }

        public static void GenerateConfigFileForAllModule(string[] assemblyFilesPath, string xmlFilePath)
        {
            var modules = new SystemHellModules();
            
            foreach (var assembly in assemblyFilesPath) {
                var loadedAssembly = Assembly.LoadFile(assembly);
                foreach (Type daemonType in loadedAssembly.GetTypes().GetInheritedInterface(typeof(IDaemonModule).Name)) {
                    
                    var daemon = new Daemon { Name = daemonType.Name, Actif = false, Assembly = loadedAssembly.FullName , ModuleType = daemonType.FullName };
                    var instanceDaemon = Activator.CreateInstance(daemonType) as IDaemonModule;
                    if( instanceDaemon == null )
                        throw new Exception("Impossible de créer une instance du démon " + daemonType.Name);
                    
                    daemon.Configuration = instanceDaemon.Configuration;
                    modules.Modules.Add(daemon);
                }                           
            }
            SaveModulesXmlFile(xmlFilePath, modules);
        }

        private static IEnumerable<Type> GetInheritedInterface(this IEnumerable<Type> AllTypes, string interfaceName)
        {
            foreach (Type t in AllTypes)
            {
                if (t.GetInterface(interfaceName) != null && !t.IsAbstract)
                    yield return t;
            }
        } 
        
        private static object DeserializeConfiguration(XmlNode[] nodes, Assembly assembly)
        {
            if (nodes == null)
                return null;

            var configTypeNode = nodes.FirstOrDefault(a => a.NodeType == XmlNodeType.Attribute && a.Name == "TypeFullName");
            if( configTypeNode == null )
                throw new Exception("Impossible de trouver l'attribut TypeFullName dans la configuration");

            var instanceType = assembly.GetType(configTypeNode.Value);
            if( instanceType == null )
                throw new Exception("Impossible de trouver le type " + configTypeNode.Value + " dans l'assembly " + assembly.FullName);

            var xmlConfig = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + "\r\n" +
                            "<" + instanceType.Name + " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + "\r\n";

            xmlConfig = nodes.Where(a => a.NodeType == XmlNodeType.Element).Aggregate(xmlConfig, (current, xmlNode) => current + (xmlNode.OuterXml + "\r\n"));

            xmlConfig += "</" + instanceType.Name + ">";

            return LoadFromXmlString(xmlConfig, instanceType);
        }

        private static object LoadFromXmlString(string content, Type type)
        {
            using (var stringReader = new StringReader(content)) {
                var xmlTextReader = new XmlTextReader(stringReader);                
                var xmlSerializer = new XmlSerializer(type);
                var result = xmlSerializer.Deserialize(xmlTextReader);
                xmlTextReader.Close();
                return result;
            }
        }

        public static SystemHellModules LoadModuleXmlFile(string xmlFilePath)
        {                        
            using (var streamReader = new StreamReader(xmlFilePath))
            {
                if( streamReader.BaseStream.Length == 0 )
                    throw new ModuleFileEmptyException(xmlFilePath);

                var xmlReader = XmlReader.Create(streamReader, new XmlReaderSettings { CheckCharacters = true });
                var xmlSerializer = new XmlSerializer(typeof(SystemHellModules));
                var result = (SystemHellModules)xmlSerializer.Deserialize(xmlReader);
                xmlReader.Close();                            
                return result;
            }            
        }
        
        public static void SaveModulesXmlFile(string xmlFilePath, SystemHellModules serializableObject)
        {
            var serialiseur = new XmlSerializer(typeof(SystemHellModules), serializableObject.GetConfigTypes());            
            XmlWriter xmlWriter = XmlWriter.Create(xmlFilePath, new XmlWriterSettings { CheckCharacters = true, Indent = true });            
            serialiseur.Serialize(xmlWriter, serializableObject);
            xmlWriter.Close();
        }

        private static Type[] GetConfigTypes(this SystemHellModules serializableObject)
        {
            if (serializableObject == null) throw new ArgumentNullException("serializableObject");
            return (from modules in serializableObject.Modules where modules.Configuration != null select modules.Configuration.GetType()).ToArray();
        }     
    }
}