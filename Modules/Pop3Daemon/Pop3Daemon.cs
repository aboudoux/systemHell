using System;
using System.Globalization;
using System.IO;
using System.Threading;
using OpenPop.Mime;
using OpenPop.Pop3;
using SharedDaemonLib;

namespace Pop3Daemon
{
    public class Pop3Daemon : DaemonBase<Pop3Configuration>
    {
        public override void Start(CancellationToken cancellationToken)
        {
            do {
                try {

                    if (!Directory.Exists(Configuration.SaveAttachementDirectory))
                        Directory.CreateDirectory(Configuration.SaveAttachementDirectory);                    

                    using (var client = new Pop3Client()) {
                        
                        WriteTrace("Connexion au serveur Pop3 {0}:{1}...", Configuration.Pop3Server, Configuration.Pop3Port.ToString(CultureInfo.InvariantCulture));
                        client.Connect(Configuration.Pop3Server, Configuration.Pop3Port, false);
                        client.Authenticate(Configuration.Pop3Login, Configuration.Pop3Password);
                        
                        int messageCount = client.GetMessageCount();
                        WriteTrace("{0} message(s) présent(s) dans la boite aux lettres de {1}", messageCount.ToString(CultureInfo.InvariantCulture), Configuration.Pop3Login);

                        for (int i = 1; i < (messageCount + 1) && !cancellationToken.IsCancellationRequested; i++) {
                            Message message = client.GetMessage(i);
                            if (!(Configuration.CheckingMailAddress.ToLower().Contains(message.Headers.From.Address.ToLower()) && message.Headers.Subject.ToLower().Contains(Configuration.CheckingSubjectTag.ToLower())))
                            {
                                WriteTrace("Suppression du message {0}", i.ToString(CultureInfo.InvariantCulture));
                                client.DeleteMessage(i);
                            }
                            else {
                                foreach (MessagePart part in message.FindAllAttachments()) {
                                    WriteTrace("Enregistrement de la pièce jointe du message {0}", i.ToString(CultureInfo.InvariantCulture));
                                    part.Save(new FileInfo(Path.Combine(Configuration.SaveAttachementDirectory, Path.GetFileNameWithoutExtension(part.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(part.FileName))));
                                    client.DeleteMessage(i);
                                }
                            }
                        }
                        WriteTrace("Fin du traitement le la boite aux lettres {0}", Configuration.Pop3Login);
                    }
                }
                catch (Exception exception) {
                    WriteTrace("BAL {0} - Erreur : {1}",Configuration.Pop3Login, exception.Message);
                }
                
            } while (!cancellationToken.WaitHandle.WaitOne(Configuration.LoopTime * 1000));            
            WriteTrace("stopped.");
        }
    }
}
