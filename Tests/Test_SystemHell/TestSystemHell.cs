using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using SystemHell;
using SystemHell.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedDaemonLib;
using Test_SystemHell.FakeModules;

namespace Test_SystemHell
{
    [TestClass]
    public class TestSystemHell
    {
        private int _stopTimeout = 1000;

        [TestMethod]
        public void TestStartAndStopFakeModuleEvent()
        {
            // Arrange
            RuntimeDaemonHostService hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleEvent();
            
            // Act
            hostService.OnStart(new List<IDaemonModule>() { module });

            var FakeModule1 = (hostService as IDaemonHostService).LoadedModules.FirstOrDefault(a => a.Item1.GetType() == typeof(FakeModuleEvent));
            Assert.IsNotNull(FakeModule1);
                        
            // Assert
            Assert.IsTrue(FakeModule1.Item2.Status == TaskStatus.Running);                                                            
            hostService.OnStop();
            Assert.IsTrue(FakeModule1.Item2.IsCompleted);
            
        }

        [TestMethod]
        public void TestStartAndStopFakeModuleUnstop()
        {
            // Arrange
            RuntimeDaemonHostService hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleUnstop();

            // Act
            hostService.OnStart(new List<IDaemonModule>() { module });

            var FakeModule1 = (hostService as IDaemonHostService).LoadedModules.FirstOrDefault(a => a.Item1.GetType() == typeof(FakeModuleUnstop));
            Assert.IsNotNull(FakeModule1);

            // Assert
            Assert.IsTrue(FakeModule1.Item2.Status == TaskStatus.Running);
            hostService.OnStop();
            Assert.IsTrue(FakeModule1.Item2.Status == TaskStatus.Running);

        }
        
        [TestMethod]
        public void TestStartAndStopFakeModuleWithCancelation()
        {
            // Arrange
            RuntimeDaemonHostService hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleWithCancelation();

            // Act
            hostService.OnStart(new List<IDaemonModule>() { module });

            var FakeModule1 = (hostService as IDaemonHostService).LoadedModules.FirstOrDefault(a => a.Item1.GetType() == typeof(FakeModuleWithCancelation));
            Assert.IsNotNull(FakeModule1);

            // Assert
            Assert.IsTrue(FakeModule1.Item2.Status == TaskStatus.Running);
            hostService.OnStop();
            Assert.IsTrue(FakeModule1.Item2.IsCompleted);

        }

        [TestMethod]
        public void TestStartAndStopFakeModuleWithNothing()
        {
            // Arrange
            RuntimeDaemonHostService hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleWithNothing();

            // Act
            hostService.OnStart(new List<IDaemonModule>() { module });

            var FakeModule1 = (hostService as IDaemonHostService).LoadedModules.FirstOrDefault(a => a.Item1.GetType() == typeof(FakeModuleWithNothing));
            Assert.IsNotNull(FakeModule1);

            // Assert            
            hostService.OnStop();
            Assert.IsTrue(FakeModule1.Item2.IsCompleted);
        }

        [TestMethod]
        [ExpectedException(typeof(StartModuleException))]
        public void TestStartAndStopFakeModuleThrowException()
        {
            // Arrange
            RuntimeDaemonHostService hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleThrowException();

            // Act            
            hostService.OnStart(new List<IDaemonModule>() { module });            
        }       
    }
}
