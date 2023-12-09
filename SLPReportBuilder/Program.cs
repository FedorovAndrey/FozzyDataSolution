using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SLPMailSender;
using SLPReportBuilder;
using SLPReportCreater;

var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
logger.LogInformation("Program has started.");

DateTime dateTime = DateTime.Now;

try
{
    var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    var configSection = configBuilder.GetSection("AppSettings");
    var reportFolder = configSection["report_folder"] ?? null;

    if (!Directory.Exists(reportFolder))
    {
        _ = Directory.CreateDirectory(reportFolder);
    }



    //ReportBuilderCore.GenerateReport(reportFolder);
    ReportBuilderCore.GenerateReports(reportFolder);



    logger.LogInformation("All threads are complete");

}
catch (Exception ex)
{
    logger.LogCritical(ex.Message);
}








