using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Events.Bus;
using Castle.Core.Logging;
using Pfizer.PTEImporter.Job;
using Pfizer.PTEImporter.Services;
using System;
using System.Configuration;
using System.IO;
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
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        public Worker(
            IEventBus eventBus,
            IBackgroundJobManager backgroundJobManager,
            IFileService fileService,
            IEmailService emailService
            )
        {
            _eventBus = eventBus;
            _backgroundJobManager = backgroundJobManager;
            _fileService = fileService;
            _emailService = emailService;

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

            //allowed to be running within the days of the week configured in app.config
            var runningDays = ConfigurationManager.AppSettings["RunningDays"];
            if (!string.IsNullOrEmpty(runningDays))
            {
                var days = runningDays.Split(',');
                var dayOfTheWeek = Convert.ToInt32(DateTime.Now.DayOfWeek);
                if (!days.Any( x => x == dayOfTheWeek.ToString()))
                {
                    Environment.Exit(0);
                }
            }
            //get the full path for the data source file
            string sourceFileFullPath = string.Empty;
            string sourceFileDirectory = string.Empty;

            var dataSourceFileDirectory = ConfigurationManager.AppSettings["DataSourceFileDirectory"];
            if (!string.IsNullOrEmpty(dataSourceFileDirectory))
            {
                sourceFileDirectory = dataSourceFileDirectory;
            }
            else
            {
                sourceFileDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "In");
            }

            DirectoryInfo directory = new DirectoryInfo(sourceFileDirectory);
            var files = directory.GetFiles();

            if (files.Length == 0)
            {
                var subject = ConfigurationManager.AppSettings["Subject"];
                var from = ConfigurationManager.AppSettings["From"];
                var recipients = ConfigurationManager.AppSettings["Recipients"];
                var tempateFullPath = string.Format(@"{0}\Template\{1}", AppDomain.CurrentDomain.BaseDirectory, "Template.html");
                string body = await _fileService.ReadAllText(tempateFullPath);

                var splittedRecipients = recipients.Split(';');
                _emailService.SendEmail(from, splittedRecipients, subject, body, new string[] { });
                Environment.Exit(0);
            }

            var sourceFile = files.ToList().FirstOrDefault();
            sourceFileFullPath = sourceFile.FullName;

            _backgroundJobManager.Enqueue<LoadDataJob, LoadDataJobParameter>(new LoadDataJobParameter { FilePath = sourceFileFullPath });
            Logger.Info("The worker finished its task, background job is running...");
        }
    }
}