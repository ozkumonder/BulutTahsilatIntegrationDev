using System.ServiceProcess;
using System.Threading;

namespace BulutTahsilatIntegration.WinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new BulutTahsilatService()
            //};
            //ServiceBase.Run(ServicesToRun);

#if DEBUG

            var main = new BulutTahsilatService();
            main.OnDebug();
            Thread.Sleep(Timeout.Infinite);
#else
                                                                                                                                                                                                                                    ServiceBase[] ServicesToRun;
                                                                                                                                                                                                                                    ServicesToRun = new ServiceBase[]
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        new ClientControlService()
                                                                                                                                                                                                                                    };
                                                                                                                                                                                                                                    ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
