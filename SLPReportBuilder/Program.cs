using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SLPReportBuilder;

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
#pragma warning disable CS8604 // Possible null reference argument.
        _ = Directory.CreateDirectory(reportFolder);
#pragma warning restore CS8604 // Possible null reference argument.
    }



    //ReportBuilderCore.GenerateReport(reportFolder);
    ReportBuilderCore.GenerateReports(reportFolder);



    logger.LogInformation("All threads are complete");

}
catch (Exception ex)
{
    logger.LogCritical(ex.Message);
}








