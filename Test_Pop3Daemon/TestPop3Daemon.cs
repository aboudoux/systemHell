using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pop3Daemon;
using SharedDaemonLib;

namespace Test_Pop3Daemon
{
    [TestClass]
    public class TestPop3Daemon
    {
        [TestMethod]
        [Ignore]
        public void TestStartDaemonOnRecoMail()
        {
            Pop3Daemon.Pop3Daemon daemon = new Pop3Daemon.Pop3Daemon();
            ((IDaemonModule) daemon).Configuration = new Pop3Configuration() {
                CheckingMailAddress = "actibasesender@gmail.com;ris@bug.fr",
                CheckingSubjectTag = "[BUG RIS]",
                Pop3Login = "bugris",
                Pop3Password = "secret",
                Pop3Server = "pop.test.fr",
                SaveAttachementDirectory = "d:\\temp\\pop3",
            };

            CancellationToken token = new CancellationToken();
            daemon.Start(token);
            Console.ReadKey();
        }
    }
}
