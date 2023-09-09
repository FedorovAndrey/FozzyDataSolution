using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using SLPDBLibrary;
using SLPHelper;
using System.Drawing;

namespace SLPReportCreater
{
    public class WorkWithExcel
    {
        private int regionId;

        private DateTime dateTime_Begin;
        private DateTime dateTime_End;

        Microsoft.Extensions.Logging.ILogger logger ;
        ExcelPackage excel;


        public WorkWithExcel(int regonId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<WorkWithExcel>();

            this.regionId = regonId;
        }
        public void Generate()
        {
            logger.LogInformation("Report generation for the region ID : " + regionId.ToString());
            try
            {
                Task.Factory.StartNew(() => GenerateReport(ReportType.Day));
                Task.Factory.StartNew(() => GenerateReport(ReportType.Week));
                Task.Factory.StartNew(() => GenerateReport(ReportType.Month));
                Task.Factory.StartNew(() => GenerateReport(ReportType.Year));

                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                {
                    //Task.Factory.StartNew(() => GenerateReport(ReportType.Week));
                }
                if (DateTime.Now.Day == 1)
                {
                    //Task.Factory.StartNew(() => GenerateReport(ReportType.Month));
                }
                if (DateTime.Now.DayOfYear == 1)
                {
                    //Task.Factory.StartNew(() => GenerateReport(ReportType.Year));
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

            #region 

            #endregion


            logger.LogInformation("The task for generating the report is complete : " + reportType.ToString());
        }

        private bool GenerateBranchReportTemplate(ref ExcelPackage package, string name)
        {
            bool bResult = false;
            try
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(name);


                using (ExcelRange range = worksheet.Cells["A2:D9"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 12, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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