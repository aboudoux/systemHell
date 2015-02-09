using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedDaemonLib;

namespace SystemHell.Service
{
    public interface IDaemonHostService
    {
        void OnStart(List<IDaemonModule> modules );
        void OnStop();

        List<Tuple<IDaemonModule, Task>> LoadedModules { get; }
    }
}
