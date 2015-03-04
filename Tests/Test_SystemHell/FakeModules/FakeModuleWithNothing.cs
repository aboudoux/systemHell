using System.Threading;
using SharedDaemonLib;
using Test_SystemHell.FakeModules.Configurations;

namespace Test_SystemHell.FakeModules
{
    public class FakeModuleWithNothing : DaemonBase<FakeConfiguration>
    {
        public override void Start(CancellationToken cancellationToken)
        {            
        }        
    }
}
