using System.Threading;
using SharedDaemonLib;
using Test_SystemHell.FakeModules.Configurations;

namespace Test_SystemHell.FakeModules
{
    public class FakeModuleUnstop : DaemonBase<FakeConfiguration>
    {        
        public override void Start(CancellationToken cancellationToken)
        {
            DaemonStarted = true;
            Thread.Sleep(-1);            
        }        
    }
}
