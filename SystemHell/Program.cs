using System.ServiceProcess;
using SystemHell.Service;

namespace SystemHell
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] 
			{ 
				new DaemonHost(new RuntimeDaemonHostService()),  
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
