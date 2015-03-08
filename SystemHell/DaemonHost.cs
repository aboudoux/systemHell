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
            Trace.WriteLine("[SystemHell] Starting...");

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
            Trace.WriteLine("[SystemHell] Stopping...");
            _service.OnStop();
            base.OnStop();
        }

        protected override void OnCustomCommand(int command)
        {
            if (command == 200) {
                Trace.WriteLine("[SystemHell] Reloading daemons...");
                var moduleConfigfile = DirectoryHelper.DaemonFilePath;
                if (!File.Exists(moduleConfigfile))
                    throw new Exception("Configuration file does not exists.");
                _service.ReloadDaemons(ModuleLoader.GetAllDaemon(moduleConfigfile));
            }
            else {
                Trace.WriteLine("[SystemHell] Custom command " + command + " received.");
                _service.OnCustomCommand(command);
            }

            base.OnCustomCommand(command);
        }

        private void InitializeComponent()
        {
            Trace.AutoFlush = true;
            ServiceName = "SystemHell";
            AutoLog = true;            
        }
    }
}
