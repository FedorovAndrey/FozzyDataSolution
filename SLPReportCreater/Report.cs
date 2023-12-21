using System.Diagnostics.Metrics;
using NLog;
using SLPDBLibrary;
using SLPDBLibrary.Models;
using SLPHelper;
using SLPMailSender;

namespace SLPReportCreater
{
    public class Report : IDisposable
    {
        private string _rootFolder = "";
#pragma warning disable CS0414 // The field 'Report._reportFolder' is assigned but its value is never used
        private string _reportFolder = "";
#pragma warning restore CS0414 // The field 'Report._reportFolder' is assigned but its value is never used

        private ReportType _reportType;
        private EnergyResource _energyResource;

        private Logger logger = LogManager.GetLogger("logger");
        private List<SLPDBLibrary.Region> regions;

        List<Thread> _threads = new List<Thread>();

        public Report(string rootFolder, ReportType reportType, EnergyResource resource)
        {
            _rootFolder = rootFolder;
            _energyResource = resource;
            _reportType = reportType;

            regions = new List<SLPDBLibrary.Region>();
        }

        public void Dispose()
        {

        }

        public void Generate()
        {
            if (!Controler.GetRegions(ref regions))
            {
                logger.Warn("Obtaining a list of regtons ended unsuccessfully. " +
                    "There are no records in the database");

            }

            if (regions != null && regions.Count > 0)
            {

                foreach (var item in regions)
                {
                    #region Creating folders for reports by region
                    if (!Helper.CreateFolderReportByRegions(_rootFolder, item.Name))
                    {

                    }
                    #endregion

                    #region Getting the list of branches for regions

                    List<BranchInformation> branches = new List<BranchInformation>();
                    if (!Controler.GetBranchesInformation(ref branches, item.ID))
                    {
                        logger.Warn("Could not get a list of branches for the regin. There is no data in the database.");
                    }

                    if (branches != null && branches.Count > 0)
                    {
                        item.Branches.AddRange(branches);

                    }
                    #endregion
                }

                foreach (var item in regions)
                {
                    if (item != null && item.Branches.Count > 0)
                    {
                        Thread tread = new Thread(() =>
                        {
                            ThreadProcess(item, _rootFolder, _reportType, _energyResource);
                        });
                        tread.Name = item.Name + ": Thread report created ";
                        _threads.Add(tread);

                    }

                }

                foreach (var thread in _threads)
                {
                    if (thread != null && !thread.IsAlive)
                    {
                        logger.Info(thread.Name + " - Is Started ...");
                        thread.IsBackground = true;
                        thread.Start();

                    }
                }

                foreach (var thread in _threads)
                {
                    if (thread.IsAlive)
                    {
                        thread.Join();
                    }
                }
            }


        }
        private void ThreadProcess(SLPDBLibrary.Region region, string rootFolder, ReportType reportType, EnergyResource energyResource)
        {

            if (energyResource == EnergyResource.Energy)
            {
                GenerateEnergyReport(region, reportType);
            }

            if (energyResource == EnergyResource.Water)
            {
                GenerateWaterReport(region, reportType);
            }


            Thread.Sleep(10);

        }

        private void GenerateEnergyReport(SLPDBLibrary.Region region, ReportType reportType)
        {
            DateTime TimestampBegin = DateTime.MinValue;
            DateTime TimestampEnd = DateTime.MaxValue;
            string path = Path.Combine(this._rootFolder, region.Name);

            string message = String.Concat(region.Name, " : Energy report - ", reportType.ToString(), " - Start generated...");
            logger.Info(message);

            try
            {
                #region Select time interval from type of report

                switch (reportType)
                {
                    case ReportType.Day:
                        TimestampBegin = DateTime.Today.AddDays(-1);
                        TimestampEnd = DateTime.Today;
                        break;

                    case ReportType.Week:
                        TimestampBegin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                        TimestampEnd = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                        break;

                    case ReportType.Month:
                        TimestampBegin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
                        TimestampEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        break;

                    case ReportType.Year:
                        TimestampBegin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
                        TimestampEnd = new DateTime(DateTime.Today.Year, 1, 1);
                        break;
                }
                region.TimestampBegin = TimestampBegin;
                region.TimestampEnd = TimestampEnd;
                
                #endregion

                #region Get data from database for branch meters
                if (region.Branches.Count > 0)


                {
                    //for (int i = 0; i < 1; i++)
                    for (int i = 0; i < region.Branches.Count; i++)
                    {
                        List<SLPDBLibrary.Meter> meters = region.Branches[i].EnergyMeters;

                        if (meters.Count > 0)
                        {
                            switch (reportType)
                            {
                                case ReportType.Day:
#pragma warning disable CS8604 // Possible null reference argument.
                                    if (!Controler.GetMeterData(ref meters, region.Branches[i].ServerName, ReportType.Day, EnergyResource.Energy, TimestampBegin, TimestampEnd))
                                    {
                                        logger.Warn(String.Concat(region.Branches[i].Address, " - The method of obtaining data on daily electricity consumption failed!"));
                                    }
#pragma warning restore CS8604 // Possible null reference argument.
                                    break;
                                case ReportType.Week:
#pragma warning disable CS8604 // Possible null reference argument.
                                    if (!Controler.GetMeterDataWeekly(ref meters, region.Branches[i].ServerName, EnergyResource.Energy, TimestampBegin, TimestampEnd))
                                    {
                                        logger.Warn(String.Concat(region.Branches[i].Address, " - The method of obtaining data on daily electricity consumption failed!"));
                                    }
#pragma warning restore CS8604 // Possible null reference argument.
                                    break;
                                case ReportType.Month:
#pragma warning disable CS8604 // Possible null reference argument.
                                    if (!Controler.GetMeterDataWeekly(ref meters, region.Branches[i].ServerName, EnergyResource.Energy, TimestampBegin, TimestampEnd))
                                    {
                                        logger.Warn(String.Concat(region.Branches[i].Address, " - The method of obtaining data on daily electricity consumption failed!"));
                                    }
#pragma warning restore CS8604 // Possible null reference argument.
                                    break;
                                case ReportType.Year:
                                    break;
                            }
                        }
                        else
                        {
                            logger.Info(String.Concat(region.Branches[i].Address, " There are no electricity consumption metering units for the branch"));
                        }

                        Thread.Sleep(10);
                    }
                }

                #endregion


                string filename = Path.Combine(path, Helper.GetFileName(region.Name, reportType.ToString(), path, EnergyResource.Energy.ToString()));
                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, reportType, EnergyResource.Energy, region);

                dailyReportWorkbook.Generate();
                dailyReportWorkbook.Save();

                using (WorkWithMail sender = new WorkWithMail())
                {
                    sender.SendReport(region.Name, filename, region.ID, reportType);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }


        }
        private void GenerateWaterReport(SLPDBLibrary.Region region, ReportType reportType)
        {
            DateTime TimestampBegin = DateTime.MinValue;
            DateTime TimestampEnd = DateTime.MaxValue;
            string path = Path.Combine(this._rootFolder, region.Name);

            string message = String.Concat(region.Name, " : Water report - ", reportType.ToString(), " - Start generated...");
            logger.Info(message);

            try
            {
                #region Select time interval from type of report

                switch (reportType)
                {
                    case ReportType.Day:
                        TimestampBegin = DateTime.Today.AddDays(-1);
                        TimestampEnd = DateTime.Today;
                        break;

                    case ReportType.Week:
                        TimestampBegin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                        TimestampEnd = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                        break;

                    case ReportType.Month:
                        TimestampBegin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
                        TimestampEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        break;

                    case ReportType.Year:
                        TimestampBegin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
                        TimestampEnd = new DateTime(DateTime.Today.Year, 1, 1);
                        break;
                }
                #endregion

                #region Get data from database for branch meters
                if (region.Branches.Count > 0)
                {
                    for (int i = 0; i < region.Branches.Count; i++)
                    {
                        List<SLPDBLibrary.Meter> meters = region.Branches[i].WaterMeters;

                        if (meters.Count > 0)
                        {
                            switch (reportType)
                            {
                                case ReportType.Day:
#pragma warning disable CS8604 // Possible null reference argument.
                                    if (!Controler.GetMeterData(ref meters, region.Branches[i].ServerName, ReportType.Day, EnergyResource.Water, TimestampBegin, TimestampEnd))
                                    {
                                        logger.Warn(String.Concat(region.Branches[i].Address, " - The method of obtaining data on daily electricity consumption failed!"));
                                    }
#pragma warning restore CS8604 // Possible null reference argument.
                                    break;
                                case ReportType.Week:
#pragma warning disable CS8604 // Possible null reference argument.
                                    if (!Controler.GetMeterDataWeekly(ref meters, region.Branches[i].ServerName, EnergyResource.Water, TimestampBegin, TimestampEnd))
                                    {
                                        logger.Warn(String.Concat(region.Branches[i].Address, " - The method of obtaining data on daily electricity consumption failed!"));
                                    }
#pragma warning restore CS8604 // Possible null reference argument.
                                    break;
                                case ReportType.Month:
#pragma warning disable CS8604 // Possible null reference argument.
                                    if (!Controler.GetMeterDataWeekly(ref meters, region.Branches[i].ServerName, EnergyResource.Water, TimestampBegin, TimestampEnd))
                                    {
                                        logger.Warn(String.Concat(region.Branches[i].Address, " - The method of obtaining data on daily electricity consumption failed!"));
                                    }
#pragma warning restore CS8604 // Possible null reference argument.
                                    break;
                                case ReportType.Year:
                                    break;
                            }

                        }
                        else
                        {
                            logger.Info(String.Concat(region.Branches[i].Address, " There are no water consumption metering units for the branch"));
                        }
                        Thread.Sleep(10);
                    }
                }

                #endregion

                logger.Info(String.Concat(region.Name, " - Creating a report file for a region"));
                string filename = Path.Combine(path, Helper.GetFileName(region.Name, reportType.ToString(), path, EnergyResource.Water.ToString()));
                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, reportType, EnergyResource.Water, region);

                dailyReportWorkbook.Generate();
                dailyReportWorkbook.Save();

                using (WorkWithMail sender = new WorkWithMail())
                {
                    sender.SendReport(region.Name, filename, region.ID, reportType);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


    }
}
