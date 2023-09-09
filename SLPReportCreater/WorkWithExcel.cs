using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OfficeOpenXml;
using SLPDBLibrary;
using SLPHelper;
using System.Drawing;

namespace SLPReportCreater
{
    public class WorkWithExcel
    {
        private int regionId;
        Microsoft.Extensions.Logging.ILogger logger ;
        private ReportType type;
        private DateTime datetimeReportBegin;
        private DateTime datetimeReportEnd;
        private List<tbRegions> regions;

        public WorkWithExcel(int regonId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<WorkWithExcel>();

            this.regionId = regonId;
        }
        

        public void Generate()
        {
            logger.LogInformation("Report generation for the region ID : " + regionId.ToString());
            for (int i = 0; i < 500; i++)
            {
            }
            logger.LogInformation("End report generation for the region ID" + regionId.ToString());
        }

        private void GenerateReportByRegion(int region) 
        {

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