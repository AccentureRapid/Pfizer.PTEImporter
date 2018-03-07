using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Net.Mail.Smtp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NPOI;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Pfizer.PTEImporter.Model;

namespace Pfizer.PTEImporter.Services
{
    public class ImporterService : IImporterService
    {
        private IEventBus _eventBus;
     
        public ImporterService()
        {
            
        }

        public async Task<List<EpayRawDataLandingModel>> ReadDataSource(string fullPath)
        {
            var result = await Task.Run(() => {

                IWorkbook workbook = null;
                FileStream stream = null;
                List<EpayRawDataLandingModel> list = new List<EpayRawDataLandingModel>();

                using (stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    // 2007version  
                    if (Path.GetExtension(fullPath) == ".xlsx")
                        workbook = new XSSFWorkbook(stream);
                    // 2003version  
                    if (Path.GetExtension(fullPath) == ".xls")
                        workbook = new HSSFWorkbook(stream);

                    ReadExcelDataToList(workbook, list);
                }
                return list;
            });

            return result;
        }

        private void ReadExcelDataToList(IWorkbook workbook, List<EpayRawDataLandingModel> list)
        {
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet != null)
            {
                int lastRowNum = sheet.LastRowNum;
                for (int i = 0; i < lastRowNum; i++)
                {
                    var model = new EpayRawDataLandingModel();
                    IRow row = sheet.GetRow(i); 
                    if (row != null)
                    {
                        int cellCount = row.LastCellNum;//total cells
                        
                        if (cellCount > 0)
                        {
                            ICell cell = row.GetCell(0);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                model.ReportId = cellValue;
                            }

                            //TODO read other cells
                            //ICell cell = row.GetCell(1);
                            //if (cell != null)
                            //{
                            //    string cellValue = cell.StringCellValue;
                            //    model.ReportId = cellValue;
                            //}
                        }

                        list.Add(model);
                    }
                }
               
            }
        }
    }
}
