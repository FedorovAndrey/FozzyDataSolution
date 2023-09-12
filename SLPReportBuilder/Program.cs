
using System.Configuration;
using NLog;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SLPReportBuilder;
using System.Globalization;
using SLPDBLibrary;
using SLPReportCreater;
using System.Drawing;

var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
logger.LogInformation("Program has started.");

DateTime dateTime = DateTime.Now;

try
{
    using (DatabaseContext db = new DatabaseContext())
    {
        var queryRegoin = (from region in db.tbRegions select region).ToList();

        foreach (var region in queryRegoin)
        {
            if (region != null)
            {
                logger.LogInformation("A report generation thread is created : " + region.Name);

                WorkWithExcel regionReport = new WorkWithExcel(region.ID, region.Name);
                Thread regionThread = new Thread(regionReport.Generate);
                regionThread.Start();
            }

        }
        
        
    }
}
catch (Exception ex)
{
    logger.LogCritical(ex.Message);
}








