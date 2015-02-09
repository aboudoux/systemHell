using System;
using System.Threading;
using SharedDaemonLib;
using Test_SystemHell.FakeModules.Configurations;

namespace Test_SystemHell.FakeModules
{
    public class FakeModuleThrowException : DaemonBase<FakeConfiguration>
    {
        public override void Start(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }       
    }
}
