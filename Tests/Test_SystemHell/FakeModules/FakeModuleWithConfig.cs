using System.Diagnostics;
using System.Threading;
using SharedDaemonLib;
using Test_SystemHell.FakeModules.Configurations;

namespace Test_SystemHell.FakeModules
{
    public class FakeModuleWithConfig : DaemonBase<TestDaemonConfiguration1>
    {
        private AutoResetEvent stopEvent = new AutoResetEvent(false);

        public override void Start(CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne();
            Trace.Write("finish");
        }       
    }
}