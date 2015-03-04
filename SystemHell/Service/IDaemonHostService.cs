using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SharedDaemonLib;

namespace SystemHell.Service
{
    public interface IDaemonHostService
    {
        void OnStart(List<IDaemonModule> modules );
        void OnStop();
        void OnCustomCommand(int command);
        void ReloadDaemons(List<IDaemonModule> modules);

        List<Tuple<IDaemonModule, Task, CancellationTokenSource>> LoadedModules { get; }
    }
}
