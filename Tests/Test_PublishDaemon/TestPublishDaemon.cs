using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublishDaemon;
using PublishDaemon.Exceptions;

namespace Test_PublishDaemon
{
    [TestClass]
    public class TestPublishDaemon
    {
        [TestMethod]            
        public void TestGetLastBuild()
        {   
            RuntimePublishService publishService = new RuntimePublishService();
            var last_build = publishService.GetLastBuild("http://srvtfs2012:8080/tfs/Collection_v7", "Actibase Radiologie 7.1", DateTime.MinValue);

            Assert.IsNotNull(last_build);
            Assert.IsFalse(string.IsNullOrEmpty(last_build.BuildNumber));
        }

        [TestMethod]
        [ExpectedException(typeof(TeamFoundationServiceUnavailableException))]
        public void TestGetLastBuildWithBadCollection()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            var last_build = publishService.GetLastBuild("http://srvtfs2012:8080/tfs/Collection_osef", "Actibase Radiologie 7.1", DateTime.MinValue);            
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectDoesNotExistWithNameException))]
        public void TestGetLastBuildWithBadProject()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            var last_build = publishService.GetLastBuild("http://srvtfs2012:8080/tfs/Collection_v7", "projet tous moisi", DateTime.MinValue);            
        }

        [TestMethod]
        public void TestGetLastBuildReturnNull()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            var last_build = publishService.GetLastBuild("http://srvtfs2012:8080/tfs/Collection_v7", "Actibase Radiologie 7.1", DateTime.MaxValue);            
            Assert.IsNull(last_build);
        }

        [TestMethod]
        public void TestCanPublishWithNullBuild()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();            

            // Act
            bool result = publishService.CanPublish(null, "2");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCanPublishWithTestStatus()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();
            FakeBuildDetail buildDetail = new FakeBuildDetail();
            FeedBuildDetail(buildDetail);
            
            // Act            
            buildDetail.TestStatus = BuildPhaseStatus.Failed;
            bool resultError = publishService.CanPublish(buildDetail, "2");

            buildDetail.TestStatus = BuildPhaseStatus.Succeeded;
            bool resultSuccess = publishService.CanPublish(buildDetail, "2");

            // Assert
            Assert.IsFalse(resultError);
            Assert.IsTrue(resultSuccess);
        }

        [TestMethod]
        public void TestCanPublishWithCompileStatus()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();
            FakeBuildDetail buildDetail = new FakeBuildDetail();
            FeedBuildDetail(buildDetail);

            // Act            
            buildDetail.CompilationStatus = BuildPhaseStatus.Failed;
            bool resultError = publishService.CanPublish(buildDetail, "2");

            buildDetail.CompilationStatus = BuildPhaseStatus.Succeeded;
            bool resultSuccess = publishService.CanPublish(buildDetail, "2");

            // Assert
            Assert.IsFalse(resultError);
            Assert.IsTrue(resultSuccess);
        }

        [TestMethod]
        public void TestCanPublishWithBuildNumber()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();
            FakeBuildDetail buildDetail = new FakeBuildDetail();
            FeedBuildDetail(buildDetail);

            // Act                        
            bool resultError = publishService.CanPublish(buildDetail, "1");            
            bool resultSuccess = publishService.CanPublish(buildDetail, "2");

            // Assert
            Assert.IsFalse(resultError);
            Assert.IsTrue(resultSuccess);
        }

        [TestMethod]
        [ExpectedException(typeof(MsBuildNotFoundException))]
        public void TestPublishWithNoMSBuild()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();            
            // Act
            publishService.Publish(@"F:\qfjln.exe", "", "", new Version());            
        }

        [TestMethod]
        [ExpectedException(typeof(BadMsbuildException))]
        public void TestPublishWithBadMSBuild()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();
            string path = CreateTempFile("Build.exe");

            try {
                publishService.Publish(path, "", "", new Version());                
            }
            finally{
                File.Delete(path);                
            }                                  
        }

        [TestMethod]
        [ExpectedException(typeof(SolutionNotFoundException))]
        public void TestPublishWithNoSolution()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();
            string MSBuildPath = CreateTempFile("MSBuild.exe");            

            try
            {
                publishService.Publish(MSBuildPath, "", "", new Version());
            }
            finally
            {
                File.Delete(MSBuildPath);
            }   
        }

        [TestMethod]
        [ExpectedException(typeof(BadSolutionException))]
        public void TestPublishWithBadSolution()
        {
            // Arrange
            RuntimePublishService publishService = new RuntimePublishService();
            string MSBuildPath = CreateTempFile("MSBuild.exe");
            string SolutionPath = CreateTempFile("Solution.txt");

            try
            {
                publishService.Publish(MSBuildPath, SolutionPath, "", new Version());
            }
            finally
            {
                File.Delete(MSBuildPath);
                File.Delete(SolutionPath);
            }  
        }

        [TestMethod]
        [ExpectedException(typeof(PublishDirectoryNotFoundException))]        
        public void AccessPublishDirectoryEmpty()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            publishService.AccessPublishDirectory(null, "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(PublishDirectoryNotFoundException))]        
        public void AccessPublishDirectoryNotFound()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            publishService.AccessPublishDirectory("c:\\lfnlsdfnk", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void AccessPublishDirectoryUNCUnfound()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            publishService.AccessPublishDirectory(@"\\osef\osef", "", "");
        }       

        [TestMethod]        
        public void AccessPublishDirectoryAlreadyConnected()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            publishService.AccessPublishDirectory(@"\\192.168.69.100\wwwroot\ARV71", "Administrateur", "1234");
        }               

         [TestMethod]         
         public void TestIncrementVersion()
         {
             RuntimePublishService publishService = new RuntimePublishService();
             var version = publishService.IncrementVersion(new Version(1, 2, 3, 4));
             Assert.AreEqual("1.2.3.5", version.ToString());
             version = publishService.IncrementVersion(new Version(1, 2, 3, 6));
             Assert.AreEqual("1.2.3.7", version.ToString());
         }

        [TestMethod]         
        public void TestDirectoryExist()
        {
            Assert.IsTrue(Directory.Exists("C:\\"));
            Assert.IsTrue(Directory.Exists("C:"));
        }


        [TestMethod]
        [Ignore]
        public void TestRealsePublish()
        {
            RuntimePublishService publishService = new RuntimePublishService();
            publishService.Publish(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe", @"D:\Dropbox\code\C#\ARV7\Global.sln", @"\\192.168.69.100\wwwroot\ARV71", new Version(3,0,0,0));
        }

        private void FeedBuildDetail(IBuildDetail buildDetail)
        {
            buildDetail.Status = BuildStatus.Succeeded;
            buildDetail.TestStatus = BuildPhaseStatus.Succeeded;
            buildDetail.CompilationStatus = BuildPhaseStatus.Succeeded;
            buildDetail.BuildNumber = "1";
        }

        private string CreateTempFile(string fileName)
        {
            string path = Path.Combine(Path.GetTempPath(), fileName);
            var tempFile = File.Create(path);
            tempFile.Close();
            return path;
        }
    }

}
