using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Events.Bus;
using Castle.Core.Logging;
using Pfizer.PTEImporter.Job;
using System;
using System.Linq;
using System.Threading;

namespace Pfizer.PTEImporter
{
    //Entry class of the test. It uses constructor-injection to get a repository and property-injection to get a Logger.
    public class Worker : ITransientDependency
    {
        public ILogger Logger { get; set; }


        private readonly IEventBus _eventBus;
        private readonly IBackgroundJobManager _backgroundJobManager;
        public Worker(
            IEventBus eventBus,
            IBackgroundJobManager backgroundJobManager
            )
        {
            _eventBus = eventBus;
            _backgroundJobManager = backgroundJobManager;

            Logger = NullLogger.Instance;
        }

        public async void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = (Exception)e.ExceptionObject;
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                Environment.Exit(1);
            };

            Logger.Info("The worker is running...");
           

            //if no interface file found, e email will send to MAIN team
            //var files = await _systemService.GetInterfaceFileInfo();
            //if (!files.Any())
            //{

                //var subject = string.Format("sample({0})", DateTime.UtcNow.ToString("dd/MM/yyyy"));
                //var tempatePath = string.Format(@"{0}\Template\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "NoInterfaceLoadedEmailTemplate.html");
                //string body = await _fileService.ReadAllText(tempatePath);
                //_emailService.SendEmailAsync(from, recipients, subject, body, new string[] { });

                //Logger.Info("Application will exit in 10 seconds...");
                //Environment.Exit(0);
            //}
          
            _backgroundJobManager.Enqueue<LoadDataJob, LoadDataJobParameter>(new LoadDataJobParameter { FilePath = "" });
            Logger.Info("The worker finished its task, background job is running...");
        }
    }
}