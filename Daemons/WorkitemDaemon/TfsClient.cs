using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace WorkitemDaemon
{
    public class TfsClient
    {
        private Project _project;
        private WorkItemStore _store;

        private int AddWorkitem(EnumWorkitemType itemType, string title, string description, string rank, IEnumerable<string> attachements)
        {
            try
            {                
                WorkItemType type = _project.WorkItemTypes[itemType.ToString()];
                var item = new WorkItem(type) {
                    Title = (title.Length > 128) ? title.Substring(0, 128) : title                    
                };
                item.Title = item.Title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
                item.Description = description.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>"); ;
                
                item["Backlog Priority"] = rank;

                if (itemType == EnumWorkitemType.Bug)
                    item["Repro Steps"] = item.Description;

                if (attachements != null) {
                    foreach (string str in attachements.Where(File.Exists)) {
                        item.Attachments.Add(new Attachment(str));
                    }
                }
                var list = item.Validate();
                if (list.Count > 0)
                {
                    string str2 = "";
                    foreach (Field field in list)
                    {
                        string str3 = str2;
                        str2 = str3 + field.Name + " - " + field.Status.ToString() + "\r\n";
                    }
                    throw new Exception("L'item que vous essayez d'ajouter n'est pas valide.\r\n" + str2);
                }
                item.Save();
                return item.Id;
            }
            catch (ConnectionException)
            {
                IsConnected = false;
            }
            catch (FileAttachmentException)
            {
                IsConnected = false;
            }
            return 0;
        }     

        public void Connect(string serverUrl, string userName, string password, string projectName)
        {
            TfsClientCredentials clientCredentials;

            if (serverUrl.StartsWith("https"))
            {
                var cred = new NetworkCredential(userName, password);
                var basicCred = new BasicAuthCredential(cred);
                clientCredentials = new TfsClientCredentials(basicCred) {AllowInteractive = false};
            }
            else {                
                clientCredentials = new TfsClientCredentials(new SimpleWebTokenCredential(userName, password));
            }

            var teamFoundationServer = new TfsTeamProjectCollection(new Uri(serverUrl), clientCredentials);
            teamFoundationServer.Authenticate();

            _store = new WorkItemStore(teamFoundationServer);
            _project = _store.Projects[projectName];
            IsConnected = true;            
        }

        public List<TfsArea> GetAreas()
        {
            return (from Node node in _project.AreaRootNodes select new TfsArea(node)).ToList();
        }

        public List<string> GetUsers()
        {
            var allowedValues = _store.FieldDefinitions[CoreField.AssignedTo].AllowedValues;
            return allowedValues.Cast<string>().ToList();
        }

        public int Send(InternalWorkItem workitem)
        {
            return AddWorkitem(workitem.Type, workitem.Titre, workitem.Description, workitem.Rank, workitem.GetAttachementFiles());
        }

        public bool IsConnected { get; private set; }
    }
}