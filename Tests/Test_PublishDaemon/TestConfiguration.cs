using System;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublishDaemon;

namespace Test_PublishDaemon
{
    [TestClass]
    public class TestConfiguration
    {
        [TestMethod]
        public void TestFirstOpenConfigFile()
        {
            // Arrange
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestFirstOpenConfigFile.config");
            PublishConfiguration.DefineConfigFile(filePath);

            Assert.AreEqual(PublishConfiguration.Collection, "http://srvtfs2012:8080/tfs/Collection_v7");
            Assert.AreEqual(PublishConfiguration.ProjectName, "Actibase Radiologie 7.1");
            Assert.AreEqual(PublishConfiguration.LastBuildTime, DateTime.MinValue);
            Assert.AreEqual(PublishConfiguration.LastBuildVersion, "");            

            File.Delete(filePath);
        }

        [TestMethod]
        public void TestSaveDataInConfig()
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestSaveDataInConfig.config");
            PublishConfiguration.DefineConfigFile(filePath);

            // Act
            PublishConfiguration.Collection = "http://test";
            PublishConfiguration.DefineConfigFile(filePath);

            // Assert
            Assert.AreEqual(PublishConfiguration.Collection, "http://test");

            File.Delete(filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(NoNullAllowedException))]
        public void TestNullDateInConfigFile()
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestNullDateInConfigFile.config");
            PublishConfiguration.DefineConfigFile(filePath);

            // Act
            try {
                PublishConfiguration.Collection = null;                
            }
            finally {
                File.Delete(filePath);
            }
        }        
    }
}
