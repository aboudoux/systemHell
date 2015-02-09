using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using SystemHell.Exceptions;
using SystemHell.Service;
using SharedDaemonLib.Helpers;

namespace SystemHell
{
    public class DaemonHost : ServiceBase
    {
        readonly IDaemonHostService _service;
        
        public DaemonHost(IDaemonHostService service)
        {            
            if (service == null) throw new ArgumentNullException("service");
            _service = service;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("Démarrage du service SystemHell");

            var moduleConfigfile = DirectoryHelper.DaemonFilePath;
            if (File.Exists(moduleConfigfile)) {
                _service.OnStart(ModuleLoader.GetAllDaemon(moduleConfigfile));
                base.OnStart(args);
            }
            else {
                ModuleLoader.GenerateConfigFileForAllModule(Directory.GetFiles(DirectoryHelper.CurrentDirectory, "*Daemon*.dll"), moduleConfigfile);
                throw new DaemonFileGeneratedException(moduleConfigfile);
            }
        }

        protected override void OnStop()
        {
            Trace.WriteLine("Arrêt du service SystemHell");
            _service.OnStop();
            base.OnStop();
        }

        private void InitializeComponent()
        {
            Trace.AutoFlush = true;
            ServiceName = "SystemHell";
            AutoLog = true;            
        }
    }
}
