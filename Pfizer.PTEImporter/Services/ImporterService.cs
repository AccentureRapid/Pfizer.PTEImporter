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
using Abp.Domain.Repositories;
using Pfizer.PTEImporter.Core.Entities;

namespace Pfizer.PTEImporter.Services
{
    public class ImporterService : IImporterService
    {
        private const int skippedRowCount = 1; // total row count should be skipped in the excel file
 
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
                for (int i = 0; i <= lastRowNum; i++)
                {
                    if (i > (skippedRowCount - 1))
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
                                    var cellValue = cell.StringCellValue;
                                    model.ReportId = cellValue.ToString();
                                }

                                model.CompanyCode = "7106";//hard code for pfizer company
                                model.VendorInvoiceRef = model.ReportId;

                                ICell cellEpayApprovedDate = row.GetCell(14);
                                if (cellEpayApprovedDate != null)
                                {
                                    var cellValue = cellEpayApprovedDate.DateCellValue;
                                    model.EpayApprovedDate = cellValue.ToString();
                                }

                                ICell cellCostCenter = row.GetCell(7);
                                if (cellCostCenter != null)
                                {
                                    var cellValue = cellCostCenter.StringCellValue;
                                    model.CostCenter = cellValue.ToString();
                                }
                                model.MaterialGroup = string.Empty;//TODO null
                                model.MaterialGroupName = string.Empty;//TODO null
                                model.MaterialCategory = string.Empty;//TODO null

                                ICell cellItemStatus = row.GetCell(17);
                                if (cellItemStatus != null)
                                {
                                    var cellValue = cellItemStatus.StringCellValue;
                                    model.ItemStatus = cellValue.ToString();
                                }

                                model.PONumber = string.Empty;//TODO
                                //ICell cellPONumber = row.GetCell(x);//TODO
                                //if (cellPONumber != null)
                                //{
                                //    var cellValue = cellPONumber.StringCellValue;
                                //    model.PONumber = cellValue.ToString();
                                //}

                                model.CreatedOn = DateTime.Now.ToString("");

                                ICell cellAmount = row.GetCell(21);
                                if (cellAmount != null)
                                {
                                    var cellValue = cellAmount.NumericCellValue;
                                    model.Amount = cellValue.ToString();
                                }

                                ICell cellEpay_Requester = row.GetCell(5);
                                if (cellEpay_Requester != null)
                                {
                                    var cellValue = cellEpay_Requester.StringCellValue;
                                    //TODO get EID by Employee ID from employee
                                    model.Epay_Requester = cellValue.ToString();
                                }

                                model.EpayNumber = model.ReportId;
                                model.InvoiceSource = string.Empty;

                                ICell cellSubject = row.GetCell(2);
                                if (cellSubject != null)
                                {
                                    var cellValue = cellSubject.StringCellValue;
                                    model.Subject = cellValue.ToString();
                                }

                                model.LineNumber = string.Empty;
                            }

                            list.Add(model);
                        } 
                    }
                }
               
            }
        }



    }
}
