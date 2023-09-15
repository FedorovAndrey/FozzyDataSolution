using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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

                List<BranchInformation> branches = new Controler().GetBranchesInformation(regionId);

                if (branches != null)
                {
                    if (!GenerateBranchListWorksheet(ref excel, branches))
                    {
                        
                    }

                    foreach(BranchInformation item in branches)
                    {
                        if (!GenerateBranchReportTemplate(ref excel, item, reportType))
                        { 

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

        private bool GenerateBranchListWorksheet(ref ExcelPackage excel, List<BranchInformation> branches)
        {
            bool bResult = false;

            try
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Branch List");

                using (ExcelRange range = worksheet.Cells["A1:A2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Філія");
                }

                using (ExcelRange range = worksheet.Cells["B1:B2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Місто");
                }

                using (ExcelRange range = worksheet.Cells["C1:C2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Адреса");
                }

                using (ExcelRange range = worksheet.Cells["D1:D2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Кількість лічильників");
                }
                using (ExcelRange range = worksheet.Cells["E1:E2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Споживання кВт*год");
                }
                using (ExcelRange range = worksheet.Cells["F1:F2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Посилання");
                }

                int startRow = 3;

                foreach (BranchInformation item in branches)
                {
                    using (ExcelRange range = worksheet.Cells[String.Concat("A", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, item.id.ToString());
                    }

                    using (ExcelRange range = worksheet.Cells[String.Concat("B", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, item.City);
                    }

                    using (ExcelRange range = worksheet.Cells[String.Concat("C", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, item.Address);
                    }

                    using (ExcelRange range = worksheet.Cells[String.Concat("D", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#";
                        range.SetCellValue(0, 0, item.meterCount);
                    }

                    using (ExcelRange range = worksheet.Cells[String.Concat("E", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.SetCellValue(0, 0, "0.00");
                    }

                    using (ExcelRange range = worksheet.Cells[String.Concat("F", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetHyperlink(new ExcelHyperLink(String.Concat(item.id,"!A1"),"Перейти"));
                    }
                    startRow++;
                }
                bResult = true;
            }
            catch(Exception ex )
            {

            }
            return bResult;
        }
        private bool GenerateBranchReportTemplate(ref ExcelPackage package, BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            DateTime dateTime = DateTime.Now;
            string reportTitle = "";
            try
            {
                switch (reportType)
                {
                    case ReportType.Day:
                        reportTitle = String.Concat("Добовий графік спожитої електроенергії за: ", dateTime_Begin.Date.ToShortDateString());
                        break;
                    case ReportType.Week:
                    case ReportType.Month:
                    case ReportType.Year:
                        reportTitle = String.Concat("Графік спожитої електроенергії з: ", dateTime_Begin.Date.ToShortDateString(), " по ", dateTime_End.Date.ToShortDateString());
                        break;
                    default:
                        reportTitle = "Графік спожитої електроенергії";
                        break;
                }

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(branch.id.ToString());

                using (ExcelRange range = worksheet.Cells["A2:D9"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 12, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat(branch.City, ", ", branch.Address));
                }

                using (ExcelRange range = worksheet.Cells["F6:I6"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_Begin.Date.ToShortDateString()));
                }

                using (ExcelRange range = worksheet.Cells["F7:I7"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_End.Date.ToShortDateString()));
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
                
                using (ExcelRange range = worksheet.Cells["F11:L11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, reportTitle);
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