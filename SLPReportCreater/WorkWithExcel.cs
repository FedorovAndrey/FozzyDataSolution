using OfficeOpenXml;
using SLPHelper;

namespace SLPReportCreater
{
    public class WorkWithExcel
    {
        private ReportType type;
        private DateTime datetimeReportBegin;
        private DateTime datetimeReportEnd;

        public WorkWithExcel(ReportType reportType)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            this.type = reportType;
            switch(type) 
            {
                case ReportType.Day:
                    datetimeReportBegin = DateTime.Now.Date.AddDays(-1);
                    datetimeReportEnd = DateTime.Now.Date.AddDays(1);   
                    break;
                case ReportType.Week:
                    break;
                case ReportType.Month:
                    break;
                case ReportType.Year:
                    break;
                default: throw new ArgumentException();
            }
        }

        public void Generate()
        { 
            DateTime dateTime = DateTime.Now;   

        }
    }

}