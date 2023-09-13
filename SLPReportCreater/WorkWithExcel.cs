using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using SLPDBLibrary;
using SLPHelper;
using System;
using System.Collections.Immutable;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Text;

namespace SLPReportCreater
{
    public class WorkWithExcel
    {
        private int regionId;
        private string regionName;

        private DateTime dateTime_Begin;
        private DateTime dateTime_End;

        Microsoft.Extensions.Logging.ILogger logger ;
        ExcelPackage excel;


        public WorkWithExcel(int regonId, string regionName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<WorkWithExcel>();

            this.regionId = regonId;
            this.regionName = regionName;
        }
        public void Generate()
        {
            logger.LogInformation("Report generation for the region ID : " + regionId.ToString());
            try
            {
                GenerateReport(ReportType.Day);




                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                {
                    GenerateReport(ReportType.Week);
                }
                if (DateTime.Now.Day == 1)
                {
                    GenerateReport(ReportType.Month);
                }
                if (DateTime.Now.DayOfYear == 1)
                {
                    GenerateReport(ReportType.Year);
                }

            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
            }
            

            logger.LogInformation("End report generation for the region ID" + regionId.ToString());
        }
        private void GenerateReport(ReportType reportType)
        {
            logger.LogInformation("A task is started to generate the report : " + reportType.ToString());

            switch (reportType)
            {
                case ReportType.Day:
                    dateTime_Begin = DateTime.Today.AddDays(-1);
                    dateTime_End = DateTime.Today;
                    break;
                case ReportType.Week:
                    dateTime_Begin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                    dateTime_End = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1)); 
                    break;
                case ReportType.Month:
                    dateTime_Begin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
                    dateTime_End = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case ReportType.Year:
                    dateTime_Begin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
                    dateTime_End = new DateTime(DateTime.Today.Year, 1, 1);
                    break;

            }

            #region Create excel Workbook
            try
            {
                FileInfo fileInfo = new FileInfo(Helper.GetFileName(regionName, reportType.ToString()));
                excel = new ExcelPackage(fileInfo);

                logger.LogInformation("Creating a report file  : " + fileInfo.FullName);

                logger.LogInformation("Execute a query to the database for selecting branches belonging to the region");

                using (DatabaseContext db = new DatabaseContext())
                {
                    var queryResult = (from branche in db.tbBranche
                                       join city in db.tbCities on branche.City equals city.ID
                                       join region in db.tbRegions on branche.Region equals region.ID
                                       where branche.Region == regionId
                                       orderby branche.ID
                                       select new
                                       {
                                           BrancheID = branche.ID,
                                           BrancheRegion = region.Name,
                                           BrancheCity = city.Name,
                                           BrancheAddress = branche.Address
                                       });
                    if (queryResult != null)
                    {
                        foreach (var item in queryResult)
                        {

                            if (!GenerateBranchReportTemplate(ref excel, item.BrancheID, String.Concat(item.BrancheCity, ", ", item.BrancheAddress)))
                            {
                                logger.LogError("Creation of a report template for a branch " + item.BrancheAddress + " completed with an error");
                            }
                        }
                    }
                }

                excel.Save();
            }
            catch(Exception ex) 
            {
                logger.LogError(ex.Message);
            }
            finally 
            { 
            }

            #endregion
            
            logger.LogInformation("The task for generating the report is complete : " + reportType.ToString());
        }

        private bool GenerateBranchListWorksheet(ref ExcelPackage excel, int branchId, string branchCity, string branchAddress, int branchMeterCount)
        {
            bool bResult = false;

            try
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Branch List");

                bResult = true;
            }
            catch(Exception ex )
            {

            }
            return bResult;
        }

        private bool GenerateBranchListWorksheet(ref ExcelPackage package, IQueryable data)
        {
            bool bResult = false;

            try
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Branch List");

                using (ExcelRange range = worksheet.Cells["A1:A2"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Філія");
                }
                using (ExcelRange range = worksheet.Cells["B1:B2"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Місто");
                }
                using (ExcelRange range = worksheet.Cells["C1:C2"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Адреса");
                }
                using (ExcelRange range = worksheet.Cells["D1:D2"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Кількість лічильників");
                }
                using (ExcelRange range = worksheet.Cells["E1:E2"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Посилання");
                }


                

                

                bResult = true;
            }
            catch (Exception ex)
            {

            }
            return bResult;
        }
        private bool GenerateBranchReportTemplate(ref ExcelPackage package, int brancheId, string brancheName)
        {
            bool bResult = false;

            DateTime dateTime = DateTime.Now;
            try
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(brancheId.ToString());

                using (ExcelRange range = worksheet.Cells["A2:D9"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 12, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0,0,brancheName);
                }

                using (ExcelRange range = worksheet.Cells["F2:G2"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, "Покази лічильників на:");
                }

                using (ExcelRange range = worksheet.Cells["F3:G3"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, "Покази лічильників на:");
                }

                using (ExcelRange range = worksheet.Cells["A11:B11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Italic = true;
                    range.SetCellValue(0, 0, "Час створення звіту:");
                }

                using (ExcelRange range = worksheet.Cells["C11:D11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Italic = true;
                    range.SetCellValue(0, 0, dateTime.Date.ToShortDateString() + " " + dateTime.ToShortTimeString());
                }

                bResult = true;
            }
            catch (Exception ex) 
            { 
            }
            return bResult;
        }
    }

}