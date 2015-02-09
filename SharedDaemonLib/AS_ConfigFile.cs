using System;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;

namespace SharedDaemonLib
{
    /// <summary>
    /// Cette classe représente un fichier de configuration.
    /// Celui ci a la particularité de pouvoir etre manipulé directement via des indexeurs
    /// et gère la concurrence sur le fichier de configuration.
    /// </summary>
    // Aurélien BOUDOUX - Mars 2010
    public class AS_ConfigFile : IDisposable
    {
        private XDocument doc;
        private string _file_path;
        private Mutex m = new Mutex();
        private FileSystemWatcher watcher;

        private const string HEADER = "Configuration";

        public string file_path { get { return _file_path; } }

        /// <summary>
        /// Indexeur permettant de creer et retrouver des clé a l'interieur de section dans le fichier de configuration
        /// </summary>
        /// <param name="args">argument permettant de creer la section voulu</param>
        /// <returns>Dans la plupart des cas, la fonction retourne un objet nullable</returns>
        /// <remarks>Exemple d'utilisation : config_file["section1", "section2", "valeur"] = 25</remarks>
        /// <remarks>int? valeur = config_file["section1", "section2", "valeur"] as int?</remarks>
        public object this[params string[] args]
        {
            get
            {
                long nbr_arg = args.LongLength;
                int element_arg_index = 0;

                XElement working_element = GetWorkingElement(out element_arg_index, args);

                // Si le working element contiens un type, alors nous travaillons sur une valeur deja existante.
                if (working_element.Attribute("Type") != null && element_arg_index == nbr_arg)
                {
                    switch (working_element.Attribute("Type").Value)
                    {
                        case "System.String": return working_element.Value;
                        case "System.Int32": return working_element.Value.ToInt();
                        case "System.Int64": return working_element.Value.ToLong(); 
                        case "System.Boolean": return working_element.Value.ToBool(); 
                        case "System.UInt64": return working_element.Value.ToUlong(); 
                        case "System.UInt32": return working_element.Value.ToUint(); 
                        case "System.DateTime": return working_element.Value.ToDateTime(); 
                        case "System.Single": return working_element.Value.ToFloat(); 

                        default:
                            {
                                throw new AS_ConfigTypeException(string.Format("Le type {0} n'est pas pris en charge par le fichier de configuration", working_element.Attribute("Type").Value));
                            }
                    }
                }
                else
                    return null;
            }
            set
            {
                if( value == null )
                    throw new NoNullAllowedException("Il est interdit de définir une valeur NULL à une propriété de configuration");

                long nbr_arg = args.LongLength;
                int element_arg_index = 0;

                XElement working_element = GetWorkingElement(out element_arg_index, args);

                // Generation de l'element final contenant la valeur
                XElement last_element = new XElement(args[nbr_arg - 1]);
                last_element.Add(new XAttribute("Type", value.GetType()));
                last_element.Add(value);

                // Si le working element contiens un type, alors nous travaillons sur une valeur deja existante.
                if (working_element.Attribute("Type") != null && element_arg_index == nbr_arg)
                    working_element.ReplaceWith(last_element);
                else
                {
                    for (int i = element_arg_index; i < nbr_arg - 1; i++)
                    {
                        XElement e = new XElement(args[i]);
                        working_element.Add(e);
                        working_element = e;
                    }
                    working_element.Add(last_element);
                }
                Save();
            }
        }

        /// <summary>
        /// Utilise le fichier de configuration par defaut.
        /// </summary>
        /// <remarks>Le fichier de configuration par defaut se nomme ActisysConfiguration.config, et se trouve dans le repertoire de l'application</remarks>
        public AS_ConfigFile()
            : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"TFSDaemon","TFSDaemon.config"))
        {
        }

        /// <summary>
        /// Ouvre ou crée un nouveau fichier de configuration XML
        /// </summary>
        /// <param name="file">Chemin vers le fichier de confirguration a utiliser</param>
        public AS_ConfigFile(string file)
        {
            if (file.Contains(".."))
                _file_path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), file);
            else
                _file_path = file;

            string directoryPath = Path.GetDirectoryName(_file_path);
            if( string.IsNullOrEmpty(directoryPath) )
                throw new Exception("Aucun repertoire n'a été specifié pour le fichier de configuration.");
            Directory.CreateDirectory(directoryPath);

            if (File.Exists(_file_path) == false)
            {
                doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement(HEADER));
                Save();
            }
            else
                doc = XDocument.Load(_file_path);

            ListenChangingFile(_file_path);
        }

        private void ListenChangingFile(string file_path)
        {
            watcher = new FileSystemWatcher(Path.GetDirectoryName(file_path), Path.GetFileName(file_path));
            watcher.Changed += watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                m.WaitOne();
                doc = XDocument.Load(e.FullPath);
            }
            catch (System.Exception) {
            }
            finally
            {
                m.ReleaseMutex();
            }
        }

        public static AS_ConfigFile OpenFromAppData(string file_name)
        {
            return new AS_ConfigFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), file_name));
        }

        /// <summary>
        /// Retirer un valeur ou une section du fichier de configuration
        /// </summary>
        /// <param name="section_path"></param>
        public void Remove(params string[] section_path)
        {
            int arg_index = 0;
            XElement working_element = GetWorkingElement(out arg_index, section_path);
            working_element.RemoveAll();
            Save();
        }

        /// <summary>
        /// Fermer le fichier de configuration
        /// </summary>
        public void Close()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Changed -= watcher_Changed;
            watcher.Dispose();
            watcher = null;
            m.Close();
            doc = null;
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Sauvegarde le fichier de configuration.
        /// La fonction utilise un mutex pour gerer la concurrence entre application qui utilise AS_ConfigFile.
        /// Si une autre application à la main sur le fichier de configuration, la fonction va rentrer dans une boucle de test
        /// jusqu'a quelle est la main, ou jusqu'a un nombre défini d'itération avant de renvoyer une exception
        /// </summary>
        private void Save()
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    try
                    {
                        m.WaitOne();
                        if (watcher != null)
                            watcher.EnableRaisingEvents = false;
                        doc.Save(_file_path);
                        return;

                    }
                    catch (System.UnauthorizedAccessException deniedException)
                    {
                        throw deniedException;
                    }
                    catch (System.Exception e)
                    {
                        Trace.WriteLine(e.Message);
                    }
                    finally
                    {
                        m.ReleaseMutex();
                        if (watcher != null)
                            watcher.EnableRaisingEvents = true;
                    }
                    Thread.Sleep(1);
                }
                throw new AS_ConfigOpenFileException(string.Format("Une autre application a pris la main sur le fichier de configuration '{0}'. Impossible d'ecrire les données dans le fichier", _file_path));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XElement GetWorkingElement(out int arg_index, params string[] args)
        {
            int element_arg_index = 0;
            XElement working_element = doc.Element(HEADER);
            XElement next = null;
            foreach (string arg in args)
            {
                if (arg.Contains(" "))
                    throw new AS_ConfigKeyNameException("Vos noms de clés ne doivent pas contenir d'espaces. Merci de donner un nom de clé sans caractère d'espacement. Le nom qui pose problème est : '" + arg + "'");

                next = working_element.Element(arg);
                if (next == null)
                    break;
                else
                    working_element = next;
                element_arg_index++;
            }

            arg_index = element_arg_index;
            return working_element;
        }
    }

    [Serializable]
    public class AS_ConfigFileException : DaemonException
    {
        public AS_ConfigFileException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
        public AS_ConfigFileException() : base() { }
        public AS_ConfigFileException(string Msg) : base(Msg) { }
        public AS_ConfigFileException(string Msg, Exception InnerException) : base(Msg, InnerException) { }
    }

    [Serializable]
    public class AS_ConfigKeyNameException : DaemonException
    {
        public AS_ConfigKeyNameException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
        public AS_ConfigKeyNameException() : base() { }
        public AS_ConfigKeyNameException(string Msg) : base(Msg) { }
        public AS_ConfigKeyNameException(string Msg, Exception InnerException) : base(Msg, InnerException) { }
    }

    [Serializable]
    public class AS_ConfigOpenFileException : DaemonException
    {
        public AS_ConfigOpenFileException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
        public AS_ConfigOpenFileException() : base() { }
        public AS_ConfigOpenFileException(string Msg) : base(Msg) { }
        public AS_ConfigOpenFileException(string Msg, Exception InnerException) : base(Msg, InnerException) { }
    }

    [Serializable]
    public class AS_ConfigTypeException : DaemonException
    {
        public AS_ConfigTypeException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
        public AS_ConfigTypeException() : base() { }
        public AS_ConfigTypeException(string Msg) : base(Msg) { }
        public AS_ConfigTypeException(string Msg, Exception InnerException) : base(Msg, InnerException) { }
    }

       // -------- "Aurélien BOUDOUX - 2009"
    public static class AS_ExtendedStringsMethods
    {
        public static bool? ToBool(this string target)
        {
            bool? result = null;
            bool tmp = false;
            if (bool.TryParse(target, out tmp))
                result = tmp;
            return result;
        }

        public static long? ToLong(this string target)
        {
            long? result = null;
            long tmp = 0;
            if (long.TryParse(target, out tmp))
                result = tmp;
            return result;
        }

        public static DateTime ToDateTime(this string target)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(target, out result);
            return result;

        }

        public static int? ToInt(this string target)
        {
            int? result = null;
            int tmp = 0;
            if (int.TryParse(target, out tmp))
                result = tmp;
            return result;
        }

        public static float? ToFloat(this string target)
        {
            float? result = null;
            float tmp = 0;
            if (float.TryParse(target, out tmp))
                result = tmp;
            return result;
        }

        public static ulong? ToUlong(this string target)
        {
            ulong? result = null;
            ulong tmp = 0;
            if (ulong.TryParse(target, out tmp))
                result = tmp;
            return result;
        }

        public static uint? ToUint(this string target)
        {
            uint? result = null;
            uint tmp = 0;
            if (uint.TryParse(target, out tmp))
                result = tmp;
            return result;
        }

        public static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            char[] letters = source.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);
            return new string(letters);
        }
    }
}
