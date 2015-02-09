using System;
using System.Collections.Generic;
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

        public List<Tuple<IDaemonModule, Task>> LoadedModules { get; private set; }
    }
}