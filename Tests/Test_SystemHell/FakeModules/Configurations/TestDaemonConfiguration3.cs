using System;
using SharedDaemonLib;

namespace Test_SystemHell.FakeModules.Configurations
{
    [Serializable]
    public class TestDaemonConfiguration3 : DaemonModuleConfiguration
    {
        public TestDaemonConfiguration3()
        {
            Object1 = new CompositedObject1();
            Object2 = new CompositedObject2();
        }

        public CompositedObject1 Object1 { get; set; }
        public CompositedObject2 Object2 { get; set; }
    }

    [Serializable]
    public class CompositedObject1
    {
        public int Member1_1 { get; set; }
        public int Member1_2 { get; set; }
    }

    [Serializable]
    public class CompositedObject2
    {
        public int Member2_1 { get; set; }
        public int Member2_2 { get; set; }
    }
}