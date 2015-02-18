using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedDaemonLib;

namespace SystemHell.Service
{
    public class RuntimeDaemonHostService : IDaemonHostService
    {        
        private readonly List<Tuple<IDaemonModule, Task>> _allModules = new List<Tuple<IDaemonModule, Task>>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();        
        private readonly int _stopTimeout;        

        List<Tuple<IDaemonModule, Task>> IDaemonHostService.LoadedModules { get { return _allModules; } }
        
        public RuntimeDaemonHostService( int stopTimeout = 60000)
        {
            Trace.AutoFlush = true;
            _stopTimeout = stopTimeout;
        }

        public void OnStart(List<IDaemonModule> modules )
        {
            if (modules == null) throw new ArgumentNullException("modules");

            modules.ForEach(a => _allModules.Add(new Tuple<IDaemonModule, Task>(a, Task.Factory.StartNew(()=>a.Start(_cancellationTokenSource.Token), _cancellationTokenSource.Token))));
                        
            _allModules.ForEach(a=>Trace.WriteLine(a.Item2.Status));            
            while (_allModules.Any(a =>a.Item2.IsFaulted == false && a.Item1.DaemonStarted == false && a.Item2.IsCompleted == false))                           
                Thread.Sleep(500);

            var exceptions = new List<Exception>();
            foreach (var errors in _allModules.Where(a => a.Item2.IsFaulted)) {                
                exceptions.Add(errors.Item2.Exception);
            }
            if (exceptions.Count > 0) {
                throw new StartModuleException("Des erreurs se sont produites lors du démarrage de certains modules", exceptions);
            }
        }

        public void OnStop()
        {                                                
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_allModules.Select(a => a.Item2).ToArray(), _stopTimeout);                                   
        }        
    }
}
