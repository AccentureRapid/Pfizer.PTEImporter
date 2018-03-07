using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;
using Pfizer.PTEImporter.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.EventHandler
{
    /// <summary>
    /// TaskCompletedEventHandler
    /// </summary>
    public class TaskCompletedEventHandler : IEventHandler<TaskCompletedEventData>, ITransientDependency
    {
        private readonly IBackgroundJobManager _backgroundJobManager;


        public ILogger Logger { get; set; }
        public TaskCompletedEventHandler(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;

            Logger = NullLogger.Instance;
        }

        public void HandleEvent(TaskCompletedEventData eventData)
        {
            Logger.Info("All jobs are done. System will exit in 10 seconds");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i + 1);
                if (i == 9)
                {
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            }
        }
    }
}
