using System;
using System.IO;
using System.Threading.Tasks;
using Ionic.Zip;
using SharedDaemonLib;
using System.Threading;
using WorkitemDaemon.Helpers;


namespace WorkitemDaemon
{
    public class WorkitemDaemonModule : DaemonBase<WorkitemConfiguration>
    {
        private readonly TfsClient _client = new TfsClient();
 
        public override void Start(CancellationToken cancellationToken)
        {   
            WriteTrace("Started.");
            StartTfsConnectionLoop(cancellationToken);
            DaemonStarted = true;
 
            do {
                if(_client.IsConnected && !cancellationToken.IsCancellationRequested)
                {
                    WriteTrace("Création des workitems...");
                    var path = Path.Combine(Configuration.WorkitemDirectory, "Extracted");
                    foreach (string fileName in Directory.EnumerateFiles(Configuration.WorkitemDirectory, "*.zip", SearchOption.TopDirectoryOnly)) {
                        try {
                            WriteTrace("Extraction de {0}", fileName);
                            using (var file = new ZipFile(fileName)) {
                                Directory.CreateDirectory(path);
                                file.ExtractAll(path);
                            }
                            foreach (string args in Directory.EnumerateFiles(path, "*.xml", SearchOption.TopDirectoryOnly)) {
                                WriteTrace("Traitement de {0}", args);
                                InternalWorkItem serializableObject = null;                                
                                XmlHelper.LoadFromXMLFile(args, ref serializableObject);
                                _client.Send(serializableObject);
                            }
                            File.Delete(fileName);
                        }
                        catch (Exception exception) {
                            WriteTrace("Erreur : {0}", exception.Message);
                        }
                        finally {
                            Directory.Delete(path, true);
                        }
                    }
                }
                else
                    WriteTrace("TFS Client not connected.");                
            } while (!cancellationToken.WaitHandle.WaitOne(Configuration.LoopTime * 1000));
            WriteTrace("Stopped.");
        }

        private void StartTfsConnectionLoop(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                do
                {
                    if (!_client.IsConnected)
                    {
                        WriteTrace("connexion au serveur TFS");
                        try
                        {
                            _client.Connect(Configuration.TfsServerUrl, Configuration.TfsUserName, Configuration.TfsPassword, Configuration.TfsProjectName);
                            WriteTrace("Connection au serveur TFS OK");
                        }
                        catch (Exception ex)
                        {
                            WriteTrace("Erreur : {0}", ex.Message);
                        }
                    }
                }
                while (!cancellationToken.WaitHandle.WaitOne(Configuration.TfsConnectionLoopTime * 1000));
                WriteTrace("TFS Connexion loop - stopped");
            }, cancellationToken);
        }
    }
}
