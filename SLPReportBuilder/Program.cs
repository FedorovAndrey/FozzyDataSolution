
using System.Configuration;
using NLog;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SLPReportBuilder;
using System.Globalization;
using SLPDBLibrary;
using SLPReportCreater;
using System.Drawing;
using System.Drawing.Printing;

var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
logger.LogInformation("Program has started.");

Controler dataController = new Controler();

DateTime dateTime = DateTime.Now;

try
{
    var regions = dataController.GetRegion();

    WorkWithExcel regionReport = new WorkWithExcel(1, "Центр");
    Thread regionThread = new Thread(regionReport.Generate);
    regionThread.Start();

    /*
     if (regions != null)
    {
        foreach (var region in regions)
        {
            logger.LogInformation("A report generation thread is created : " + region.Name);
            WorkWithExcel regionReport = new WorkWithExcel(region.ID, region.Name);
            Thread regionThread = new Thread(regionReport.Generate);
            regionThread.Start();
        }

    }
     */

}
catch (Exception ex)
{
    logger.LogCritical(ex.Message);
}








