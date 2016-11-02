using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DoSoMessageSendService
{
    [RunInstaller(true)]
    public class CustomServiceInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;
        public CustomServiceInstaller()
        {
            process = new ServiceProcessInstaller();

            process.Account = ServiceAccount.LocalSystem;

            service = new ServiceInstaller();
            service.ServiceName = Program.InstallServiceName;

            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
