using System;
using SharedDaemonLib;

namespace Pop3Daemon
{
    [Serializable]
    public class Pop3Configuration : DaemonModuleConfiguration
    {
        private string _pop3Server = string.Empty;
        private string _pop3Login = string.Empty;
        private string _pop3Password = string.Empty;
        private string _checkingMailAddress = string.Empty;
        private string _checkingSubjectTag = string.Empty;
        private string _saveAttachementDirectory = string.Empty;
        private int _loopTime = 60;
        private int _pop3Port = 110;

        public string Pop3Server
        {
            get { return _pop3Server; }
            set { _pop3Server = value; }
        }

        public int Pop3Port
        {
            get { return _pop3Port; }
            set { _pop3Port = value; }
        }

        public string Pop3Login
        {
            get { return _pop3Login; }
            set { _pop3Login = value; }
        }

        public string Pop3Password
        {
            get { return _pop3Password; }
            set { _pop3Password = value; }
        }

        public string CheckingMailAddress
        {
            get { return _checkingMailAddress; }
            set { _checkingMailAddress = value; }
        }

        public string CheckingSubjectTag
        {
            get { return _checkingSubjectTag; }
            set { _checkingSubjectTag = value; }
        }

        public string SaveAttachementDirectory
        {
            get { return _saveAttachementDirectory; }
            set { _saveAttachementDirectory = value; }
        }

        public int LoopTime
        {
            get { return _loopTime; }
            set { _loopTime = value; }
        }
    }
}