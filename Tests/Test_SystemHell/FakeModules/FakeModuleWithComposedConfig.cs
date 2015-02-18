using System.Diagnostics;
using System.Threading;
using SharedDaemonLib;
using Test_SystemHell.FakeModules.Configurations;

namespace Test_SystemHell.FakeModules
{
    public class FakeModuleWithComposedConfig : DaemonBase<TestDaemonConfiguration3>
    {
        private AutoResetEvent stopEvent = new AutoResetEvent(false);

        public override void Start(CancellationToken cancellationToken)
        {
            DaemonStarted = true;
            cancellationToken.WaitHandle.WaitOne();
            Trace.Write("finish");
        }
    }
}