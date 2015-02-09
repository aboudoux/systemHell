using System;
using SharedDaemonLib;

namespace Test_SystemHell.FakeModules.Configurations
{
    [Serializable]    
    public class TestDaemonConfiguration1 : DaemonModuleConfiguration
    {
        private string _param1 = string.Empty;
        public string Param1
        {
            get { return _param1; }
            set { _param1 = value; }
        }

        public int Param2 { get; set; }
    }
}