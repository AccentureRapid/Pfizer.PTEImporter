using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Pfizer.PTEImporter.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Job
{
    public class LoadDataJob : BackgroundJob<LoadDataJobParameter>, ITransientDependency
    {
     
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEventBus _eventBus;

        public LoadDataJob(
         
            IUnitOfWorkManager unitOfWorkManager,
            IEventBus eventBus
            )
        {
 
            _unitOfWorkManager = unitOfWorkManager;
            _eventBus = eventBus;
        }
        public async override void Execute(LoadDataJobParameter data)
        {
            //1. load data from interface file
            //Logger.Info("Interface data begin loading...");
            //var lines = await _fileService.ReadTextFile(data.FilePath);
            //Logger.Info(string.Format("Interface data {0} lines loaded", lines.Count));
            //2. convert text data into data object
            //var tasks = lines.Select(async (x) =>
            //{
            //    var row = await _labourRateSetupService.ConvertToDataRow(x);
            //    return row;
            //});

            //Logger.Info("Interface data rows begin converting...");
            //var rows = tasks.Select(t => t.Result).ToList();
            //Logger.Info(string.Format("Interface data {0} rows converted", rows.Count));

            //3 batch insert these row into database.
            //var batchSaveTasks = rows.Select(
            //     async (x) =>
            //     {
            //         var row = await _costCentreGradeJobRateRepository.InsertAndGetIdAsync(
            //             new IfcCostCentreGradeJobRate
            //             {
            //                 Id = Guid.NewGuid(),
            //                 OUCostCentre = x.OUCostCentre,
            //                 JobCode = x.JobCode,
            //                 Grade = x.Grade,
            //                 StaffType = x.StaffType,
            //                 StaffCount = x.StaffCount,
            //                 AvgSal = x.AvgSal,
            //                 AVGAllowance = x.AVGAllowance,
            //                 AdjustedWorkingHours = x.AdjustedWorkingHours
            //             });
            //         return row;
            //     }
            //    );


            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            //Logger.Info("Interface data begin saving to database...");

            //var result = batchSaveTasks.Select(t => t.Result).ToList();

            //stopWatch.Stop();
            //TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //                     ts.Hours, ts.Minutes, ts.Seconds,
            //                     ts.Milliseconds / 10);

            //Logger.Info(string.Format("Interface data {0} saved to database with {1}", result.Count, elapsedTime));
            
          
       
            //5. trigger Job Completed event
            _eventBus.Trigger(new TaskCompletedEventData {  });
        }
    }
}
