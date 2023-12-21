using NLog;
using SLPDBLibrary;
using SLPHelper;
using SLPMailSender;
using SLPReportCreater;

namespace SLPReportBuilder
{
    public static class ReportBuilderCore
    {
        private static Logger logger = LogManager.GetLogger("logger");
        private static List<SLPDBLibrary.Region> _regions = new List<SLPDBLibrary.Region>();

#pragma warning disable CS0169 // The field 'ReportBuilderCore.regionId' is never used
        private static int regionId;
#pragma warning restore CS0169 // The field 'ReportBuilderCore.regionId' is never used
#pragma warning disable CS0169 // The field 'ReportBuilderCore.regionName' is never used
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static string regionName;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS0169 // The field 'ReportBuilderCore.regionName' is never used
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static string _rootFolderName;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static string _reportFolderByRegion;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private static List<Thread> _threads = new List<Thread>();


        public static void GenerateReports(string path)
        {
            try
            {
                Report dailyEnergyReport = new Report(path, ReportType.Day, EnergyResource.Energy);
                Report dailyWaterReport = new Report(path, ReportType.Day, EnergyResource.Water);

                Thread dailyEnergyReportThread = new Thread(() =>
                {
                    TreadProcess(dailyEnergyReport);
                });

                Thread dailyWaterReportThread = new Thread(() =>
                {
                    TreadProcess(dailyWaterReport);
                });

                dailyEnergyReportThread.Name = "dailyEnergyReportThread";
                dailyWaterReportThread.Name = "dailyWaterReportThread";

                _threads.Add(dailyEnergyReportThread);
                _threads.Add(dailyWaterReportThread);

                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                {
                    Report weeklyEnergyReport = new Report(path, ReportType.Week, EnergyResource.Energy);
                    Report weeklyWaterReport = new Report(path, ReportType.Week, EnergyResource.Water);

                    Thread weeklyEnergyReportThread = new Thread(() =>
                    {
                        TreadProcess(weeklyEnergyReport);
                    });
                    Thread weeklyWaterReportThread = new Thread(() =>
                    {
                        TreadProcess(weeklyWaterReport);
                    });

                    weeklyEnergyReportThread.Name = "weeklyEnergyReportThread";
                    weeklyWaterReportThread.Name = "weeklyWaterReportThread";


                    _threads.Add(weeklyEnergyReportThread);
                    _threads.Add(weeklyWaterReportThread);

                }

                if (DateTime.Now.Day == 1)
                {
                    Report monthlyEnergyReport = new Report(path, ReportType.Month, EnergyResource.Energy);
                    Report monthlyWaterReport = new Report(path, ReportType.Month, EnergyResource.Water);

                    Thread monthlyEnergyReportThread = new Thread(() =>
                    {
                        TreadProcess(monthlyEnergyReport);
                    });
                    Thread monthlyWaterReportThread = new Thread(() =>
                    {
                        TreadProcess(monthlyWaterReport);
                    });

                    monthlyEnergyReportThread.Name = "monthlyEnergyReportThread";
                    monthlyWaterReportThread.Name = "monthlyWaterReportThread";

                    _threads.Add(monthlyEnergyReportThread);
                    _threads.Add(monthlyWaterReportThread);
                }
                
                foreach (Thread thread in _threads)
                {
                    if (thread != null && !thread.IsAlive)
                    {
                        thread.Start();
                    }
                }

                foreach (Thread thread in _threads)
                {
                    if (thread != null && thread.IsAlive)
                    {
                        logger.Info(thread.Name + "Joined .........");
                        thread.Join();
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private static void TreadProcess(Report report)
        {
            try
            {
                report.Generate();

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }

//        private static void GenerateEnergyReport(SLPDBLibrary.Region region, string path)
//        {
//            logger.Info("Run of energy consumption report generation flow - " + region.Name);

//            string filename = "";
//            try
//            {
//                #region Create daily report

//                region.TimestampBegin = DateTime.Today.AddDays(-1);
//                region.TimestampEnd = DateTime.Today;

//                foreach (var branch in region.Branches)
//                {
//                    logger.Info(String.Concat(branch.Address, " - Electricity Report generation"));

//                    List<Meter> meters = branch.EnergyMeters;

//                    if (meters.Count > 0)
//                    {

//#pragma warning disable CS8604 // Possible null reference argument.
//                        if (!Controler.GetMeterData(ref meters, branch.ServerName, ReportType.Day, EnergyResource.Energy, region.TimestampBegin, region.TimestampEnd))
//                        {
//                            logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on daily electricity consumption failed!"));
//                        }
//#pragma warning restore CS8604 // Possible null reference argument.
//                    }
//                    else
//                    {
//                        logger.Info(String.Concat("There are no electricity consumption metering units for the branch: ", branch.Address));
//                    }
//                    Thread.Sleep(50);
//                }

//                logger.Info(String.Concat(region.Name, " - Creating a report file for a region"));

//                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Energy.ToString()));
//                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, ReportType.Day, EnergyResource.Energy, region);

//                dailyReportWorkbook.Generate();
//                dailyReportWorkbook.Save();

//                #endregion

//                #region Create Weekly report

//                if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
//                {
//                }

//                region.TimestampBegin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
//                region.TimestampEnd = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));

//                foreach (var branch in region.Branches)
//                {
//                    List<Meter> meters = branch.EnergyMeters;

//                    if (meters.Count > 0)
//                    {
//#pragma warning disable CS8604 // Possible null reference argument.
//                        if (!Controler.GetMeterDataWeekly(ref meters, branch.ServerName, EnergyResource.Energy, region.TimestampBegin, region.TimestampEnd))
//                        {
//                            logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on weekly electricity consumption failed!"));
//                        }
//#pragma warning restore CS8604 // Possible null reference argument.
//                    }
//                    else
//                    {
//                        logger.Info(String.Concat(branch.Address, " - There are no electricity consumption metering units for the branch: "));
//                    }

//                    Thread.Sleep(100);
//                }

//                logger.Info(String.Concat(region.Name, " - Creating a weekly report file for a region"));

//                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Energy.ToString()));
//                WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(filename, ReportType.Week, EnergyResource.Energy, region);

//                weeklyReportWorkbook.Generate();
//                weeklyReportWorkbook.Save();

//                #endregion

//                #region Create Monthly report

//                //if (DateTime.Today.Day == 1)
//                //{
//                //timestamp_begin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
//                //timestamp_end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

//                //    filename = Helper.GetFileName(region.Name, ReportType.Month.ToString(), path, EnergyResource.Energy.ToString());

//                //    WorkWithExcel monthlyReportWorkbook = new WorkWithExcel(filename);

//                //    if (!monthlyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
//                //    {

//                //    }

//                //    foreach (var branch in region.Branches)
//                //    {
//                //        #region 
//                //        if (!monthlyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
//                //        {
//                //        }
//                //        #endregion
//                //    }
//                //    monthlyReportWorkbook.Save();
//                //}
//                #endregion

//                #region Create Early report

//                //if (DateTime.Today.Day == 1 && DateTime.Today.Month == 1)
//                //{
//                //timestamp_begin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
//                //timestamp_end = new DateTime(DateTime.Today.Year, 1, 1);

//                //    filename = Helper.GetFileName(region.Name, ReportType.Year.ToString(), path, EnergyResource.Energy.ToString());

//                //    WorkWithExcel YearReportWorkbook = new WorkWithExcel(filename);

//                //    if (!YearReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
//                //    {

//                //    }

//                //    foreach (var branch in region.Branches)
//                //    {
//                //        #region 
//                //        if (!YearReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
//                //        {
//                //        }
//                //        #endregion
//                //    }
//                //    YearReportWorkbook.Save();
//                //}
//                #endregion



//            }
//            catch (Exception ex)
//            {

//                logger.Error(ex.Message);
//                logger.Error(ex.Source);
//            }
//        }

//        private static void GenerateWaterReport(SLPDBLibrary.Region region, string path)
//        {
//            logger.Info("Run of water consumption report generation flow - " + region.Name);

//            string filename = "";
//            try
//            {
//                #region Create daily report

//                region.TimestampBegin = DateTime.Today.AddDays(-1);
//                region.TimestampEnd = DateTime.Today;

//                foreach (var branch in region.Branches)
//                {
//                    logger.Info(String.Concat(branch.Address, " - Water Report generation"));

//                    List<Meter> meters = branch.WaterMeters;

//                    if (meters.Count > 0)
//                    {
//#pragma warning disable CS8604 // Possible null reference argument.
//                        if (!Controler.GetMeterData(ref meters, branch.ServerName, ReportType.Day, EnergyResource.Water, region.TimestampBegin, region.TimestampEnd))
//                        {
//                            logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on daily water consumption failed!"));
//                        }
//#pragma warning restore CS8604 // Possible null reference argument.
//                    }
//                    else
//                    {
//                        logger.Warn(String.Concat(branch.Address, " - There are no water consumption metering units for the branch"));
//                    }

//                    Thread.Sleep(50);
//                }

//                logger.Info(String.Concat(region.Name, " - Creating a report file"));

//                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Water.ToString()));
//                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, ReportType.Day, EnergyResource.Water, region);

//                dailyReportWorkbook.Generate();
//                dailyReportWorkbook.Save();

//                #endregion

//                #region Create Weekly report

//                if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
//                {
//                }

//                region.TimestampBegin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
//                region.TimestampEnd = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));



//                foreach (var branch in region.Branches)
//                {

//                    List<Meter> meters = branch.WaterMeters;

//                    if (meters.Count > 0)
//                    {
//#pragma warning disable CS8604 // Possible null reference argument.
//                        if (!Controler.GetMeterDataWeekly(ref meters, branch.ServerName, EnergyResource.Water, region.TimestampBegin, region.TimestampEnd))
//                        {
//                            logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on weekly water consumption failed!"));
//                        }
//#pragma warning restore CS8604 // Possible null reference argument.
//                    }
//                    else
//                    {
//                        logger.Warn(String.Concat(branch.Address, " - There are no water consumption metering units for the branch"));
//                    }

//                    Thread.Sleep(100);
//                }

//                logger.Info(String.Concat(region.Name, " - Creating a weekly report file for region"));

//                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Energy.ToString()));
//                WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(filename, ReportType.Week, EnergyResource.Energy, region);

//                weeklyReportWorkbook.Generate();
//                weeklyReportWorkbook.Save();

//                #endregion

//                #region Create Monthly report

//                //if (DateTime.Today.Day == 1)
//                //{
//                //timestamp_begin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
//                //timestamp_end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

//                //    filename = Helper.GetFileName(region.Name, ReportType.Month.ToString(), path, EnergyResource.Energy.ToString());

//                //    WorkWithExcel monthlyReportWorkbook = new WorkWithExcel(filename);

//                //    if (!monthlyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
//                //    {

//                //    }

//                //    foreach (var branch in region.Branches)
//                //    {
//                //        #region 
//                //        if (!monthlyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
//                //        {
//                //        }
//                //        #endregion
//                //    }
//                //    monthlyReportWorkbook.Save();
//                //}
//                #endregion

//                #region Create Early report

//                //if (DateTime.Today.Day == 1 && DateTime.Today.Month == 1)
//                //{
//                //timestamp_begin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
//                //timestamp_end = new DateTime(DateTime.Today.Year, 1, 1);

//                //    filename = Helper.GetFileName(region.Name, ReportType.Year.ToString(), path, EnergyResource.Energy.ToString());

//                //    WorkWithExcel YearReportWorkbook = new WorkWithExcel(filename);

//                //    if (!YearReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
//                //    {

//                //    }

//                //    foreach (var branch in region.Branches)
//                //    {
//                //        #region 
//                //        if (!YearReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
//                //        {
//                //        }
//                //        #endregion
//                //    }
//                //    YearReportWorkbook.Save();
//                //}
//                #endregion



//            }
//            catch (Exception ex)
//            {

//                logger.Error(ex.Message);
//                logger.Error(ex.Source);
//            }
//        }

        

    }
}
