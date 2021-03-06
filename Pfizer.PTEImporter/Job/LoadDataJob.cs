﻿using Abp.BackgroundJobs;
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
using System.Configuration;

namespace Pfizer.PTEImporter.Job
{
    [UnitOfWork(IsDisabled = true)]
    public class LoadDataJob : BackgroundJob<LoadDataJobParameter>, ITransientDependency
    {
     
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEventBus _eventBus;
        private readonly IImporterService _importerService;
        private readonly IRepository<EpayRawDataLanding,Guid> _epayRawDataLandingRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IFileService _fileService;
        private readonly IIocManager _iocManager;

        public PTEImporterDbContext Db
        {
            get
            {
                return _iocManager.ResolveAsDisposable<PTEImporterDbContext>().Object;
            }
        }
        public LoadDataJob(
            IUnitOfWorkManager unitOfWorkManager,
            IEventBus eventBus,
            IImporterService importerService,
            IRepository<EpayRawDataLanding, Guid> epayRawDataLandingRepository,
            IRepository<Employee> employeeRepository,
            IFileService fileService,
            IIocManager iocManager
            )
        {
 
            _unitOfWorkManager = unitOfWorkManager;
            _eventBus = eventBus;
            _importerService = importerService;
            _epayRawDataLandingRepository = epayRawDataLandingRepository;
            _employeeRepository = employeeRepository;
            _fileService = fileService;
            _iocManager = iocManager;
        }
        
        public async override void Execute(LoadDataJobParameter data)
        {
            //1. load data from interface file
            Logger.Info("excel data begin loading...");
            var rows = await _importerService.ReadDataSource(data.FilePath);

            var codes = rows.Select(x => x.Epay_Requester).ToList().Distinct();
            var employee = Db.Employee.Where(x => codes.Contains(x.Code)).ToList();
               
            var updatedCollection = await Task.Run(() =>
            {
                var updatedRows = rows.Select(row =>
                {
                    row.Epay_Requester = employee.FirstOrDefault(x => x.Code == row.Epay_Requester) == null ?
                    string.Empty : employee.FirstOrDefault(x => x.Code == row.Epay_Requester).LogonId;
                    return row;
                }).ToList();


                return updatedRows;
            });

           
            //2. batch insert these row into temp table.
            var batchSaveTasks = updatedCollection.Select(
                 async (x) =>
                 {
                     var entity = x.MapTo<EpayRawDataLanding>();
                     var row = await _epayRawDataLandingRepository.InsertAsync(entity);
                     Logger.Info(string.Format("report id: {0}, Id: {1} has been saved.", entity.ReportId, row.Id));
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

            //backup the source file and remove it from the source path
            Logger.Info("excel file archiving...");
            var backupFolder = ConfigurationManager.AppSettings["BackUpDirectory"];
            if (string.IsNullOrEmpty(backupFolder))
                backupFolder = string.Format(@"{0}\Backup\", AppDomain.CurrentDomain.BaseDirectory);
            FileInfo file = new FileInfo(data.FilePath);
            var destination = Path.Combine(backupFolder, file.Name);
            await _fileService.MakeSureDirectoryExist(backupFolder);

            //1.Copy interface file to destination folder
            await _fileService.Copy(data.FilePath, destination);
            //2.Remove the source file
            await _fileService.Delete(data.FilePath);

            Logger.Info("excel file archived...");

            //5. trigger Job Completed event
            _eventBus.Trigger(new TaskCompletedEventData {  });
        }
    }
}
