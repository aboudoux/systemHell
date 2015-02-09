using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SystemHell;
using SystemHell.Service;
using SharedDaemonLib;

namespace ConsoleTest
{
    class Program
    {
        private static void Main(string[] args)
        {            
            RuntimeDaemonHostService runtimeDaemon = new RuntimeDaemonHostService();
            runtimeDaemon.OnStart(ModuleHelper.GetAllDaemon());

            Console.WriteLine("Started !");
            Console.ReadKey();
        }
    }
}
