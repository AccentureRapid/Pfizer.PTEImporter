﻿using Abp;
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
                var config = string.Format(@"{0}\log4net.config", AppDomain.CurrentDomain.BaseDirectory);
                bootstrapper.IocManager
                    .IocContainer
                    .AddFacility<LoggingFacility>(f => f.LogUsing<Log4NetLoggerFactory>().WithConfig(config));

                bootstrapper.Initialize();




                //Getting a worker object from DI and running it
                using (var worker = bootstrapper.IocManager.ResolveAsDisposable<Worker>())
                {
                    worker.Object.Run();
                } //Disposes worker and all it's dependencies

                Console.WriteLine("Background job is running please do not press enter to exit...");
                Console.ReadLine();
            }

        }
    }
}
