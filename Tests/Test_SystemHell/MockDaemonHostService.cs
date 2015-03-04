using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SystemHell.Service;
using SharedDaemonLib;

namespace Test_SystemHell
{
    public class MockDaemonHostService : IDaemonHostService
    {
        public void OnStart(List<IDaemonModule> modules)
        {
        }

        public void OnStop()
        {
        }

        public void OnCustomCommand(int command)
        {            
        }

        public void ReloadDaemons(List<IDaemonModule> modules)
        {            
        }

        public List<Tuple<IDaemonModule, Task, CancellationTokenSource>> LoadedModules { get; private set; }
    }
}