using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Pfizer.PTEImporter.Core.Entities;
using Pfizer.PTEImporter.Events;
using Pfizer.PTEImporter.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.AutoMapper;

namespace Pfizer.PTEImporter.Job
{
    public class LoadDataJob : BackgroundJob<LoadDataJobParameter>, ITransientDependency
    {
     
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEventBus _eventBus;
        private readonly IImporterService _importerService;
        private readonly IRepository<EpayRawDataLanding> _epayRawDataLandingRepository;
        public LoadDataJob(
            IUnitOfWorkManager unitOfWorkManager,
            IEventBus eventBus,
            IImporterService importerService,
            IRepository<EpayRawDataLanding> epayRawDataLandingRepository
            )
        {
 
            _unitOfWorkManager = unitOfWorkManager;
            _eventBus = eventBus;
            _importerService = importerService;
            _epayRawDataLandingRepository = epayRawDataLandingRepository;
        }
        public async override void Execute(LoadDataJobParameter data)
        {
            //1. load data from interface file
            Logger.Info("excel data begin loading...");
            var rows = await _importerService.ReadDataSource(data.FilePath);

            //2. batch insert these row into temp table.
            var batchSaveTasks = rows.Select(
                 async (x) =>
                 {
                     var entity = x.MapTo<EpayRawDataLanding>();
                     var row = await _epayRawDataLandingRepository.InsertAsync(entity);
                     return row;
                 }
                );


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Logger.Info("data begin saving to temp table...");

            var result = batchSaveTasks.Select(t => t.Result).ToList();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                 ts.Hours, ts.Minutes, ts.Seconds,
                                 ts.Milliseconds / 10);

            Logger.Info(string.Format("data {0} saved to temp table with {1}", result.Count, elapsedTime));



            //5. trigger Job Completed event
            _eventBus.Trigger(new TaskCompletedEventData {  });
        }
    }
}
