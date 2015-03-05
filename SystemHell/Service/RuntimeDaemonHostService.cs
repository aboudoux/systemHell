using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemHell.Exceptions;
using SharedDaemonLib;

namespace SystemHell.Service
{
    public class RuntimeDaemonHostService : IDaemonHostService
    {
        private readonly Dictionary<string, Tuple<IDaemonModule, Task, CancellationTokenSource>> _allModules = new Dictionary<string, Tuple<IDaemonModule, Task, CancellationTokenSource>>();
        private readonly int _stopTimeout;       

        List<Tuple<IDaemonModule, Task, CancellationTokenSource>> IDaemonHostService.LoadedModules { get { return _allModules.Values.ToList(); } }
        
        public RuntimeDaemonHostService( int stopTimeout = 60000)
        {
            Trace.AutoFlush = true;
            _stopTimeout = stopTimeout;
        }

        public void OnStart(List<IDaemonModule> modules )
        {
            if (modules == null) throw new ArgumentNullException("modules");

            if( modules.Any(a=>string.IsNullOrWhiteSpace(a.ModuleName)) )
                throw new DaemonEmptyNameException();

            modules.ForEach(a => {
                if( _allModules.ContainsKey(a.ModuleName) )
                    throw new DaemonDuplicateNameException(a.ModuleName);
                var cancellationTokenSource = new CancellationTokenSource();
                _allModules.Add(a.ModuleName, new Tuple<IDaemonModule, Task, CancellationTokenSource>(a, Task.Factory.StartNew(() => a.Start(cancellationTokenSource.Token), cancellationTokenSource.Token), cancellationTokenSource));
            });
                        
            while (_allModules.Values.Any(a =>a.Item2.IsFaulted == false && a.Item1.DaemonStarted == false && a.Item2.IsCompleted == false))                           
                Thread.Sleep(100);

            var exceptions = new List<Exception>();
            foreach (var errors in _allModules.Values.Where(a => a.Item2.IsFaulted)) {                
                exceptions.Add(errors.Item2.Exception);
            }
            if (exceptions.Count > 0) {
                throw new StartModuleException(exceptions);
            }
        }

        public void OnStop()
        {
            ((IDaemonHostService)this).LoadedModules.ForEach(a => a.Item3.Cancel());
            Task.WaitAll(_allModules.Values.Select(a => a.Item2).ToArray(), _stopTimeout);                                   
        }

        public void OnCustomCommand(int command)
        {            
            ((IDaemonHostService) this).LoadedModules.ForEach(a=>a.Item1.OnCustomCommand(command));
        }

        public void ReloadDaemons(List<IDaemonModule> modules)
        {           
            var equalityComparer = new DaemonModuleEqualityComparer();

            Trace.WriteLine("[SystemHell] Stopping removed daemons");
            var moduleRemoved = ((IDaemonHostService)this).LoadedModules.Select(a => a.Item1).Except(modules, equalityComparer).ToList();
            var moduleToStops = ((IDaemonHostService)this).LoadedModules.Where(a => moduleRemoved.Contains(a.Item1, equalityComparer)).ToList();
            moduleToStops.ForEach(a => a.Item3.Cancel());
            Task.WaitAll(moduleToStops.Select(a => a.Item2).ToArray(), _stopTimeout);
            moduleToStops.ForEach(a=>_allModules.Remove(a.Item1.ModuleName));

            Trace.WriteLine("[SystemHell] Starting appended daemons");
            var moduleAdded = modules.Except(((IDaemonHostService)this).LoadedModules.Select(a => a.Item1), equalityComparer).ToList();
            OnStart(moduleAdded);

            Trace.WriteLine("[SystemHell] Reload end.");
        }
    }

    public class DaemonModuleEqualityComparer : IEqualityComparer<IDaemonModule>
    {
        public bool Equals(IDaemonModule x, IDaemonModule y)
        {
            return x.ModuleName == y.ModuleName;
        }

        public int GetHashCode(IDaemonModule obj)
        {
            return obj.ModuleName.GetHashCode();
        }
    }
}
