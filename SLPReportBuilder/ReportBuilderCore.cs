using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLPReportCreater;
using SLPHelper;

namespace SLPReportBuilder
{
    internal static class ReportBuilderCore
    {
        public static void GenerateDailyReport()
        {
            try
            {
                WorkWithExcel dailyReport = new WorkWithExcel(ReportType.Day);

                if (dailyReport != null)
                {
                    Thread dailyReportThread = new Thread(dailyReport.Generate);
                    dailyReportThread.Start();
                }
                
            }
            catch (Exception ex)
            { 
            }
        }
        public static void GenerateWeeklyReport()
        {
            try
            {

            }
            catch (Exception ex)
            {
            }
        }
        public static void GenerateMonthlyReport()
        {
            try
            {

            }
            catch (Exception ex)
            {
            }
        }

        internal static void GenerateYearlyReport()
        {
            
        }
    }
}
