using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using SLPDBLibrary;
using SLPReportCreater;
using SLPMailSender;
using System.Drawing;

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

    var regions = Controler.GetRegion();

    if (regions != null)
    {
        foreach (var region in regions)
        {
            logger.LogInformation("A report generation thread is created : " + region.Name);
            WorkWithExcel regionReport = new WorkWithExcel(region.ID, region.Name, reportFolder);
            Thread regionThread = new Thread(regionReport.Generate);
            regionThread.Start();
            
        }
    }
    
    logger.LogInformation("All threads are complete");

    //using (WorkWithMail mailSender = new WorkWithMail())
    //{
    //    mailSender.GetConfig();
    //    _ = mailSender.SendMailAsync("interandry@gmail.com", "TEST SENDER SLP REPORTS", "TEST Sender report sender");
    //}
}
catch (Exception ex)
{
    logger.LogCritical(ex.Message);
}








