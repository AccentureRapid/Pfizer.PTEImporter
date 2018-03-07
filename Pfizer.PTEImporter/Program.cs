using Abp;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Castle.Facilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            //Bootstrapping ABP system
            using (var bootstrapper = AbpBootstrapper.Create<PTEImporterModule>())
            {
                bootstrapper.IocManager
                    .IocContainer
                    .AddFacility<LoggingFacility>(f => f.LogUsing<Log4NetLoggerFactory>().WithConfig("log4net.config"));

                bootstrapper.Initialize();

                //Getting a Tester object from DI and running it
                using (var worker = bootstrapper.IocManager.ResolveAsDisposable<Worker>())
                {
                    worker.Object.Run();
                } //Disposes tester and all it's dependencies

                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }

        }
    }
}
