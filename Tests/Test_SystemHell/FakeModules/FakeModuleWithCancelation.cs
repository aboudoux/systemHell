using System.Threading;
using SharedDaemonLib;
using Test_SystemHell.FakeModules.Configurations;

namespace Test_SystemHell.FakeModules
{
    public class FakeModuleWithCancelation : DaemonBase<FakeConfiguration>
    {
        public override void Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) {
                Thread.Sleep(100);
            }
        }
    }
}
