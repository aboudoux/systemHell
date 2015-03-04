using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using SystemHell;
using SystemHell.Exceptions;
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
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleEvent();
            module.ModuleName = "test";
            
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
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleUnstop();
            module.ModuleName = "test";

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
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleWithCancelation();
            module.ModuleName = "test";

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
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleWithNothing();
            module.ModuleName = "test";

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
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleThrowException();
            module.ModuleName = "test";

            // Act            
            hostService.OnStart(new List<IDaemonModule>() { module });            
        }

        [TestMethod]
        [ExpectedException(typeof(DaemonEmptyNameException))]
        public void TestStartModuleWithEmptyName()
        {
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleWithNothing();
            module.ModuleName = string.Empty;
            hostService.OnStart(new List<IDaemonModule>() { module });
        }

        [TestMethod]
        [ExpectedException(typeof(DaemonDuplicateNameException))]
        public void TestStartModulesWithSamesName()
        {
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module1 = new FakeModuleWithNothing();
            var module2 = new FakeModuleWithNothing();

            module1.ModuleName = "TEST";
            module2.ModuleName = "TEST";
            hostService.OnStart(new List<IDaemonModule>() { module1, module2 });
        }

        [TestMethod]
        public void TestOneDaemonWithCustomCommand()
        {
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var module = new FakeModuleWithCustomCommand();
            module.ModuleName = "COMMAND";

            hostService.OnStart(new List<IDaemonModule>(){ module });
            
            hostService.OnCustomCommand(15);
            Assert.AreEqual(module.LastCustomCommandCalled, 15);

            hostService.OnCustomCommand(10);
            Assert.AreEqual(module.LastCustomCommandCalled, 10);
        }

        [TestMethod]
        public void TestManyDaemonWithCustomCommand()
        {
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
           
            var daemonModules = new List<IDaemonModule>();
            var r = new Random(DateTime.Now.Millisecond);
            var num = r.Next(30);
            
            for (int i = 0; i < num; i++) {
                var module = new FakeModuleWithCustomCommand();
                module.ModuleName = "Daemon " + i;
                daemonModules.Add(module);
            }

            hostService.OnStart(daemonModules);
            hostService.OnCustomCommand(5);

            Assert.IsTrue(daemonModules.All(a => (a as FakeModuleWithCustomCommand).LastCustomCommandCalled == 5));
        }

        [TestMethod]
        public void TestReloadAddModule()
        {
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var modules = new List<IDaemonModule>();
            
            modules.Add(new FakeModuleWithNothing(){ ModuleName = "D1"});
            hostService.OnStart(modules);

            modules.Add(new FakeModuleWithNothing() { ModuleName = "D2" });
            hostService.ReloadDaemons(modules);

            Assert.AreEqual((hostService as IDaemonHostService).LoadedModules.Count, 2);
        }
        [TestMethod]
        public void TestReloadRemoveModule()
        {
            var hostService = new RuntimeDaemonHostService(stopTimeout: _stopTimeout);
            var modules = new List<IDaemonModule>();
            
            modules.Add(new FakeModuleWithNothing(){ ModuleName = "D1"});
            modules.Add(new FakeModuleWithNothing() {ModuleName = "D2"});

            hostService.OnStart(modules);

            modules.RemoveAt(0);
            hostService.ReloadDaemons(modules);
            Assert.AreEqual((hostService as IDaemonHostService).LoadedModules.Count, 1);
            Assert.AreEqual((hostService as IDaemonHostService).LoadedModules[0].Item1.ModuleName, "D2");

        }
        [TestMethod]
        public void TestReloadUpdateModule()
        {

        }
    }
}
