using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace SystemHell
{
    [RunInstaller(true)]
    public partial class WindowsServiceInstaller : Installer
    {
        public WindowsServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information

            serviceInstaller.DisplayName = "SystemHell";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.Description = "Service permettant d'heberger et configurer des démons systèmes";

            //# This must be identical to the WindowsService.ServiceBase name

            //# set in the constructor of WindowsService.cs

            serviceInstaller.ServiceName = "SystemHell";

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
