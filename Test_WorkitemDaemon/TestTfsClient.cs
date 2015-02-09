using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedDaemonLib.Helpers;
using WorkitemDaemon;

namespace Test_WorkitemDaemon
{
    [TestClass]
    public class TestTfsClient
    {
        private const string ServeurUrl = "http://srvtfs2012:8080/tfs/Collection_V7/";
        private const string UserName = "tfssetup";
        private const string Password = "!Actibase!";
        private const string Project = "systemhell";        

        private TfsClient GetConnectedClient()
        {
            var client = new TfsClient();
            try {
                client.Connect(ServeurUrl, UserName, Password, Project);
                return client;
            }
            catch (Exception) {
                Assert.Inconclusive("Connexion au serveur TFS impossible.");
            }
            finally {
                if( !client.IsConnected )
                    Assert.Inconclusive("Impossible de se connecter au serveur TFS.");
            }
            return null;
        }

        [TestMethod]
        public void TestConnexion()
        {
            var client = GetConnectedClient();
            Assert.IsTrue(client.IsConnected);
        }

        [TestMethod]
        public void TestCreateWorkItem()
        {
            var client = GetConnectedClient();            
            var workItem = new InternalWorkItem(EnumWorkitemType.Bug, "Essai", "ceci est un test\r\ndeuxieme ligne\ntroisième ligne", "1");
            var id = client.Send(workItem);
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void TestCreateWorkItemWithAttachement()
        {
            var client = GetConnectedClient();            
            var workItem = new InternalWorkItem(EnumWorkitemType.Bug, "Test attachement", "Voici un test avec un fichier attaché", "1", new List<string>(){ Path.Combine(DirectoryHelper.CurrentDirectory, "ressources", "attachement1.bmp") });
            var id = client.Send(workItem);
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void TestCreateTaskWorkItem()
        {
            var client = GetConnectedClient();
            var workItem = new InternalWorkItem(EnumWorkitemType.Task, "Essai", "ceci est un test", "1");
            var id = client.Send(workItem);
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void TestCreateTaskWorkItemWithAttachement()
        {
            var client = GetConnectedClient();
            var workItem = new InternalWorkItem(EnumWorkitemType.Task, "Test attachement", "Voici un test avec un fichier attaché", "1", new List<string>() { Path.Combine(DirectoryHelper.CurrentDirectory, "ressources", "attachement1.bmp") });
            var id = client.Send(workItem);
            Assert.IsTrue(id > 0);
        }


        [TestMethod]
        [Ignore]
        public void TestCreateWorkitemWithTfsOnline()
        {
            var client = new TfsClient();
            try
            {
                client.Connect("https://actimedia.visualstudio.com/DefaultCollection/", "aurelien69100", "!Acti2007!", "ReconnaissanceVocale");
                var workItem = new InternalWorkItem(EnumWorkitemType.Bug, "Test attachement", "Voici un test avec un fichier attaché", "1", new List<string>() { Path.Combine(DirectoryHelper.CurrentDirectory, "ressources", "attachement1.bmp") });
                var id = client.Send(workItem);
                Assert.IsTrue(id > 0);                
            }
            catch (Exception)
            {
                Assert.Inconclusive("Connexion au serveur TFS impossible.");
            }
            finally
            {
                if (!client.IsConnected)
                    Assert.Inconclusive("Impossible de se connecter au serveur TFS.");
            }            
        }
    }
}
