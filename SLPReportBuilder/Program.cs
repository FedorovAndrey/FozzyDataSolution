
using System.Configuration;
using NLog;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SLPReportBuilder;
using System.Globalization;

var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
logger.LogInformation("Program has started.");

DateTime dateTime = DateTime.Now;

ReportBuilderCore.GenerateDailyReport();

if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
{
    ReportBuilderCore.GenerateWeeklyReport();
}
if (DateTime.Now.Day == 1)
{
    ReportBuilderCore.GenerateMonthlyReport();
}
if ((DateTime.Now.Month == 1) && (DateTime.Now.Day == 1))
{
    ReportBuilderCore.GenerateYearlyReport();
}




