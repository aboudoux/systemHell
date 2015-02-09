using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test_SystemHell
{
    [TestClass]
    public class TestApprentissage
    {
        [TestMethod]
        public void TestThreadCancellationToken()
        {
            var token = new CancellationTokenSource();
            var thread = Task.Factory.StartNew(() => {
                token.Token.WaitHandle.WaitOne();                
            }, token.Token);
            
            token.Cancel();
            Thread.Sleep(20);
            Assert.AreEqual(thread.IsCanceled, true);
        }
    }
}
