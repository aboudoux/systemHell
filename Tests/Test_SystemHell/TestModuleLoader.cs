using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SystemHell;
using SystemHell.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedDaemonLib;
using SharedDaemonLib.Helpers;
using Test_SystemHell.FakeModules.Configurations;
using Test_SystemHell.ToolBox;

namespace Test_SystemHell
{
    [TestClass]
    public class TestModuleLoader
    {
        [Ignore]
        [ExpectedException(typeof(ModuleFileNotFoundException))]
        public void TestLoadBadXmlDescription()
        {
            ModuleLoader.GetAllDaemon("d:\\osef\\" + Guid.NewGuid() + ".xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ModuleFileEmptyException))]
        public void TestLoadEmptyXmlDescription()
        {
            using (var xml = new XmlCreator()) {
                ModuleLoader.GetAllDaemon(xml.XmlFilePath);
            }
        }

        [TestMethod]
        public void TestMakeXml()
        {
            using (var xml = new XmlCreator()) {

                var modules = new SystemHellModules();
                var d1 = new Daemon() {Name = "Demon1", Actif = true, Assembly = "Pop3Daemon.dll"};
                var d2 = new Daemon() {Name = "Demon2", Actif = false, Assembly = "TestDaemon.dll"};
                d1.Configuration = new TestDaemonConfiguration1() {Param1 = "coucou", Param2 = 3};
                d2.Configuration = new TestDaemonConfiguration2() {ParamX1 = "essai", ParamX2 = 5};

                modules.Modules.Add(d1);
                modules.Modules.Add(d2);

                ModuleLoader.SaveModulesXmlFile(xml.XmlFilePath, modules);
                var result = ModuleLoader.LoadModuleXmlFile(xml.XmlFilePath);

                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Modules.Count);
                Assert.IsTrue(result.Modules.Any(a => a.Name == "Demon1"));
                Assert.IsTrue(result.Modules.Any(a => a.Name == "Demon2"));
                Assert.IsTrue(result.Modules.Any(a => a.Actif));
                Assert.IsTrue(result.Modules.Any(a => a.Assembly == "TestDaemon.dll"));
            }
        }        

        [TestMethod]
        public void TestLoadModuleFromXmlFile()
        {
            using (var xml = new XmlCreator())
            {
                var modules = new SystemHellModules();
                var d1 = new Daemon() { Name = "Test DAEMON", Actif = true, Assembly = "Test_SystemHell", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithConfig" };                               

                modules.Modules.Add(d1);                

                ModuleLoader.SaveModulesXmlFile(xml.XmlFilePath, modules);
                var result = ModuleLoader.GetAllDaemon(xml.XmlFilePath);

                Assert.AreEqual(1, result.Count);
            }
        }

        [TestMethod]
        public void TestLoadModuleFromXmlFileWithInjectedConfiguration()
        {
            using (var xml = new XmlCreator())
            {
                var modules = new SystemHellModules();
                var d1 = new Daemon() { Name = "Test DAEMON", Actif = true, Assembly = "Test_SystemHell", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithConfig" };

                d1.Configuration = new TestDaemonConfiguration1() { Param1 = "coucou", Param2 = 3 };

                modules.Modules.Add(d1);

                ModuleLoader.SaveModulesXmlFile(xml.XmlFilePath, modules);
                var result = ModuleLoader.GetAllDaemon(xml.XmlFilePath);

                Assert.AreEqual(1, result.Count);
                Assert.IsNotNull(result[0].Configuration);
                Assert.IsInstanceOfType(result[0].Configuration, typeof(TestDaemonConfiguration1));                
            }
        }

        [TestMethod]
        public void TestLoadModuleFromXmlFileWithInjectedConfigurationComposition()
        {
            using (var xml = new XmlCreator())
            {
                var modules = new SystemHellModules();
                var d1 = new Daemon() { Name = "Test DAEMON", Actif = true, Assembly = "Test_SystemHell", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithComposedConfig" };

                d1.Configuration = new TestDaemonConfiguration3();

                modules.Modules.Add(d1);

                ModuleLoader.SaveModulesXmlFile(xml.XmlFilePath, modules);
                var result = ModuleLoader.GetAllDaemon(xml.XmlFilePath);

                Assert.AreEqual(1, result.Count);
                Assert.IsNotNull(result[0].Configuration);
                Assert.IsInstanceOfType(result[0].Configuration, typeof(TestDaemonConfiguration3));
            }
        }

        [TestMethod]
        public void TestCreateDefaultConfigFile()
        {
            using (var xml = new XmlCreator()) 
            {
                List<string> dlls = new List<string>();
                dlls.Add(Path.Combine(DirectoryHelper.CurrentDirectory, "Test_SystemHell.dll"));
                ModuleLoader.GenerateConfigFileForAllModule(dlls.ToArray(), xml.XmlFilePath);
            }
        }

        [TestMethod]
        public void TestLoadInactivedModules()
        {
            using (var xml = new XmlCreator())
            {
                var modules = new SystemHellModules();
                var d1 = new Daemon() { Name = "Demon1", Actif = true, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithNothing" };
                var d2 = new Daemon() { Name = "Demon2", Actif = false, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.Configurations.FakeConfiguration" };
                
                modules.Modules.Add(d1);
                modules.Modules.Add(d2);

                ModuleLoader.SaveModulesXmlFile(xml.XmlFilePath, modules);
                var result = ModuleLoader.GetAllDaemon(xml.XmlFilePath);

                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DaemonEmptyNameException))]
        public void TestLoadModulesWithNoName()
        {
            using (var xml = new XmlCreator())
            {
                var modules = new SystemHellModules();
                var d1 = new Daemon() { Name = "A", Actif = true, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithNothing" };
                var d2 = new Daemon() { Name = "B", Actif = true, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithNothing" };
                var d3 = new Daemon() { Name = "", Actif = true, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithNothing" };                

                modules.Modules.Add(d1);
                modules.Modules.Add(d2);
                modules.Modules.Add(d3);

                ModuleLoader.SaveModulesXmlFile(xml.XmlFilePath, modules);
                var result = ModuleLoader.GetAllDaemon(xml.XmlFilePath);                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DaemonDuplicateNameException))]
        public void TestLoadModulesWithSameName()
        {
            using (var xml = new XmlCreator())
            {
                var modules = new SystemHellModules();
                var d1 = new Daemon() { Name = "A", Actif = true, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithNothing" };
                var d2 = new Daemon() { Name = "B", Actif = true, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithNothing" };
                var d3 = new Daemon() { Name = "A", Actif = true, Assembly = "Test_SystemHell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ModuleType = "Test_SystemHell.FakeModules.FakeModuleWithNothing" };

                modules.Modules.Add(d1);
                modules.Modules.Add(d2);
                modules.Modules.Add(d3);

                ModuleLoader.SaveModulesXmlFile(xml.XmlFilePath, modules);
                var result = ModuleLoader.GetAllDaemon(xml.XmlFilePath);
            }
        }
    }
}
