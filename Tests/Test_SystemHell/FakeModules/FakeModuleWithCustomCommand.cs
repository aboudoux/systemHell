using System.Threading;
using SharedDaemonLib;
using Test_SystemHell.FakeModules.Configurations;

namespace Test_SystemHell.FakeModules
{
    public class FakeModuleWithCustomCommand : DaemonBase<FakeConfiguration>
    {
        public int LastCustomCommandCalled { get; private set; }

        public override void Start(CancellationToken cancellationToken)
        {
            DaemonStarted = true;
            cancellationToken.WaitHandle.WaitOne();
        }

        public override void OnCustomCommand(int command)
        {
            LastCustomCommandCalled = command;
        }
    }
}