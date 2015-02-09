using System;
using SharedDaemonLib;

namespace WorkitemDaemon
{
    [Serializable]
    public class WorkitemConfiguration : DaemonModuleConfiguration
    {
        private int _tfsConnectionLoopTime = 1;
        private int _loopTime = 60;
        private string _workitemDirectory = string.Empty;
        private string _tfsServerUrl = string.Empty;
        private string _tfsUserName = string.Empty;
        private string _tfsPassword = string.Empty;
        private string _tfsProjectName = string.Empty;

        public int TfsConnectionLoopTime
        {
            get { return _tfsConnectionLoopTime; }
            set { _tfsConnectionLoopTime = value; }
        }

        public int LoopTime
        {
            get { return _loopTime; }
            set { _loopTime = value; }
        }

        public string WorkitemDirectory
        {
            get { return _workitemDirectory; }
            set { _workitemDirectory = value; }
        }

        public string TfsServerUrl
        {
            get { return _tfsServerUrl; }
            set { _tfsServerUrl = value; }
        }

        public string TfsUserName
        {
            get { return _tfsUserName; }
            set { _tfsUserName = value; }
        }

        public string TfsPassword
        {
            get { return _tfsPassword; }
            set { _tfsPassword = value; }
        }      

        public string TfsProjectName
        {
            get { return _tfsProjectName; }
            set { _tfsProjectName = value; }
        }
    }
}