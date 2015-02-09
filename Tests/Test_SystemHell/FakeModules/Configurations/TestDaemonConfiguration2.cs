using System;
using SharedDaemonLib;

namespace Test_SystemHell.FakeModules.Configurations
{
    [Serializable]
    public class TestDaemonConfiguration2 : DaemonModuleConfiguration
    {
        public string ParamX1 { get; set; }
        public int ParamX2 { get; set; }
    }
}