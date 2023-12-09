using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog;
using OfficeOpenXml.Table.PivotTable;
using Org.BouncyCastle.Asn1.Cmp;
using SLPDBLibrary;
using SLPDBLibrary.Models;
using SLPHelper;
using SLPMailSender;
using SLPReportCreater;

namespace SLPReportBuilder
{
    public static class ReportBuilderCore
    {
        private static Logger logger = LogManager.GetLogger("logger");
        private static List<SLPDBLibrary.Region> _regions = new List<SLPDBLibrary.Region>();

        private static int regionId;
        private static string regionName;
        private static string _rootFolderName;
        private static string _reportFolderByRegion;

        private static List<Thread> _threads = new List<Thread>();

        public static async void GenerateReport(string path)
        {
            _rootFolderName = path;
            try
            {
                logger.Info("Executing the report generation method");

                var regions = Controler.GetRegion();

                if (regions != null)
                {
                    _regions.AddRange(regions);

                    foreach (var item in _regions)
                    {
                        #region Creating folders for reports by region
                        if (!Helper.CreateFolderReportByRegions(_rootFolderName, item.Name))
                        { 

                        }
                        
                        #endregion
                    }
                }

                logger.Info("Getting the list of branches for regions");
                foreach (var region in _regions)
                {
                    List<BranchInformation> branches = new List<BranchInformation>();

                    if (!Controler.GetBranchesInformation(ref branches, region.ID))
                    {
                        logger.Warn("Could not get a list of branches for the regin. There is no data in the database.");
                    }

                    if (branches != null && branches.Count > 0)
                    {
                        region.Branches.AddRange(branches);

                    }
                }
                
                _reportFolderByRegion = Path.Combine(_rootFolderName, _regions[0].Name);

                Thread energyReportBuilder = new Thread(() =>
                {
                    GenerateEnergyReport(_regions[0], _reportFolderByRegion);
                });
                                
                Thread waterReportBuilder = new Thread(() =>
                {
                    GenerateWaterReport(_regions[0], _reportFolderByRegion);
                });

                energyReportBuilder.Start();
                waterReportBuilder.Start();

                energyReportBuilder.Join();
                waterReportBuilder.Join();

                //foreach (var region in _regions)
                //{
                //    //logger.Info("Create a thread for generating a power consumption report" + region.Name);
                //    //Thread energyReportBuilder = new Thread(() => {
                //    //    GenerateEnergyReport(region);
                //    //});

                //    //logger.Info("Create a thread for generating a water consumption report" + region.Name);
                //    //Thread waterReportBuilder = new Thread(() => {

                //    //    GenerateWaterReport(region);
                //    //});

                //    //energyReportBuilder.Name = "EnergyReportRegion#"+ region.ID.ToString();
                //    //waterReportBuilder.Name = "WaterReportRegion#" + region.ID.ToString();


                //    //energyReportBuilder.Start();
                //    //waterReportBuilder.Start() ;
                //}

                foreach (var region in _regions)
                {
                    SendReportToMailAsync(region);

                   
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        public static void GenerateReports(string path)
        {
            try
            {
                Report dailyEnergyReport = new Report(path, ReportType.Day, EnergyResource.Energy);
                Report dailyWaterReport = new Report(path, ReportType.Day, EnergyResource.Water);

                Report weeklyEnergyReport = new Report(path, ReportType.Week, EnergyResource.Energy);
                Report weeklyWaterReport = new Report(path, ReportType.Week, EnergyResource.Water);

                Report monthlyEnergyReport = new Report(path, ReportType.Month, EnergyResource.Energy);
                Report monthlyWaterReport = new Report(path, ReportType.Month, EnergyResource.Water);

                Thread dailyEnergyReportThread = new Thread(() =>
                {
                    TreadProcess(dailyEnergyReport);
                });

                Thread dailyWaterReportThread = new Thread(() =>
                {
                    TreadProcess(dailyWaterReport);
                });

                Thread weeklyEnergyReportThread = new Thread(() =>
                {
                    TreadProcess(weeklyEnergyReport);
                });

                Thread weeklyWaterReportThread = new Thread(() =>
                {
                    TreadProcess(weeklyWaterReport);
                });

                Thread monthlyEnergyReportThread = new Thread(() =>
                {
                    TreadProcess(monthlyEnergyReport);
                });

                Thread monthlyWaterReportThread = new Thread(() =>
                {
                    TreadProcess(monthlyWaterReport);
                });

                dailyEnergyReportThread.Name = "dailyEnergyReportThread";
                dailyWaterReportThread.Name = "dailyWaterReportThread";

                weeklyEnergyReportThread.Name = "weeklyEnergyReportThread";
                weeklyWaterReportThread.Name = "weeklyWaterReportThread";

                monthlyEnergyReportThread.Name = "monthlyEnergyReportThread";
                monthlyWaterReportThread.Name = "monthlyWaterReportThread";

                _threads.Add(dailyEnergyReportThread);
                _threads.Add(dailyWaterReportThread);

                _threads.Add(weeklyEnergyReportThread);
                _threads.Add(weeklyWaterReportThread);

                _threads.Add(monthlyEnergyReportThread);
                _threads.Add(monthlyWaterReportThread);


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
                        thread.Join();
                    }
                }

            }
            catch(Exception ex) 
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

        private static void GenerateEnergyReport(SLPDBLibrary.Region region, string path)
        {
            logger.Info("Run of energy consumption report generation flow - " + region.Name);

            string filename = "";
            try
            {
                #region Create daily report

                region.TimestampBegin = DateTime.Today.AddDays(-1);
                region.TimestampEnd = DateTime.Today;

                foreach (var branch in region.Branches)
                {
                    logger.Info(String.Concat(branch.Address, " - Electricity Report generation"));

                    List<Meter> meters = branch.EnergyMeters;

                    if (meters.Count > 0)
                    {

                        if (!Controler.GetMeterData(ref meters, branch.ServerName, ReportType.Day, EnergyResource.Energy, region.TimestampBegin, region.TimestampEnd))
                        {
                            logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on daily electricity consumption failed!"));
                        }
                    }
                    else
                    {
                        logger.Info(String.Concat("There are no electricity consumption metering units for the branch: ", branch.Address));
                    }
                    Thread.Sleep(50);
                }

                logger.Info(String.Concat(region.Name, " - Creating a report file for a region"));

                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Energy.ToString()));
                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, ReportType.Day, EnergyResource.Energy, region);

                dailyReportWorkbook.Generate();
                dailyReportWorkbook.Save();

                #endregion

                #region Create Weekly report

                if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                { 
                }

                    region.TimestampBegin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                    region.TimestampEnd = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));

                    foreach (var branch in region.Branches)
                    {
                        List<Meter> meters = branch.EnergyMeters;

                        if (meters.Count > 0)
                        {
                            if (!Controler.GetMeterDataWeekly(ref meters, branch.ServerName, EnergyResource.Energy, region.TimestampBegin, region.TimestampEnd))
                            {
                                logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on weekly electricity consumption failed!"));
                            }
                        }
                        else
                        {
                        logger.Info(String.Concat(branch.Address, " - There are no electricity consumption metering units for the branch: "));
                        }

                        Thread.Sleep(100);
                    }

                    logger.Info(String.Concat(region.Name," - Creating a weekly report file for a region"));

                    filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Energy.ToString()));
                    WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(filename, ReportType.Week, EnergyResource.Energy, region);

                    weeklyReportWorkbook.Generate();
                    weeklyReportWorkbook.Save();
                
                #endregion

                #region Create Monthly report

                //if (DateTime.Today.Day == 1)
                //{
                //timestamp_begin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
                //timestamp_end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                //    filename = Helper.GetFileName(region.Name, ReportType.Month.ToString(), path, EnergyResource.Energy.ToString());

                //    WorkWithExcel monthlyReportWorkbook = new WorkWithExcel(filename);

                //    if (!monthlyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                //    {

                //    }

                //    foreach (var branch in region.Branches)
                //    {
                //        #region 
                //        if (!monthlyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
                //        {
                //        }
                //        #endregion
                //    }
                //    monthlyReportWorkbook.Save();
                //}
                #endregion

                #region Create Early report

                //if (DateTime.Today.Day == 1 && DateTime.Today.Month == 1)
                //{
                //timestamp_begin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
                //timestamp_end = new DateTime(DateTime.Today.Year, 1, 1);

                //    filename = Helper.GetFileName(region.Name, ReportType.Year.ToString(), path, EnergyResource.Energy.ToString());

                //    WorkWithExcel YearReportWorkbook = new WorkWithExcel(filename);

                //    if (!YearReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                //    {

                //    }

                //    foreach (var branch in region.Branches)
                //    {
                //        #region 
                //        if (!YearReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
                //        {
                //        }
                //        #endregion
                //    }
                //    YearReportWorkbook.Save();
                //}
                #endregion



            }
            catch (Exception ex)
            {

                logger.Error(ex.Message);
                logger.Error(ex.Source);
            }
        }

        private static void GenerateWaterReport(SLPDBLibrary.Region region, string path)
        {
            logger.Info("Run of water consumption report generation flow - " + region.Name);

            string filename = "";
            try
            {
                #region Create daily report

                region.TimestampBegin = DateTime.Today.AddDays(-1);
                region.TimestampEnd = DateTime.Today;

                foreach (var branch in region.Branches)
                {
                    logger.Info(String.Concat(branch.Address, " - Water Report generation"));

                    List<Meter> meters = branch.WaterMeters;

                    if (meters.Count > 0)
                    {
                        if (!Controler.GetMeterData(ref meters, branch.ServerName, ReportType.Day, EnergyResource.Water, region.TimestampBegin, region.TimestampEnd))
                        {
                            logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on daily water consumption failed!"));
                        }
                    }
                    else
                    {
                        logger.Warn(String.Concat(branch.Address, " - There are no water consumption metering units for the branch"));
                    }

                    Thread.Sleep(50);
                }

                logger.Info(String.Concat(region.Name, " - Creating a report file"));

                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Water.ToString()));
                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, ReportType.Day, EnergyResource.Water, region);

                dailyReportWorkbook.Generate();
                dailyReportWorkbook.Save();

                #endregion

                #region Create Weekly report

                if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                {
                }

                region.TimestampBegin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                region.TimestampEnd = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));



                foreach (var branch in region.Branches)
                {

                    List<Meter> meters = branch.WaterMeters;

                    if (meters.Count > 0)
                    {
                        if (!Controler.GetMeterDataWeekly(ref meters, branch.ServerName, EnergyResource.Water, region.TimestampBegin, region.TimestampEnd))
                        {
                            logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on weekly water consumption failed!"));
                        }
                    }
                    else
                    {
                        logger.Warn(String.Concat(branch.Address, " - There are no water consumption metering units for the branch"));
                    }

                    Thread.Sleep(100);
                }

                logger.Info(String.Concat(region.Name, " - Creating a weekly report file for region"));

                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Energy.ToString()));
                WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(filename, ReportType.Week, EnergyResource.Energy, region);

                weeklyReportWorkbook.Generate();
                weeklyReportWorkbook.Save();

                #endregion

                #region Create Monthly report

                //if (DateTime.Today.Day == 1)
                //{
                //timestamp_begin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
                //timestamp_end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                //    filename = Helper.GetFileName(region.Name, ReportType.Month.ToString(), path, EnergyResource.Energy.ToString());

                //    WorkWithExcel monthlyReportWorkbook = new WorkWithExcel(filename);

                //    if (!monthlyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                //    {

                //    }

                //    foreach (var branch in region.Branches)
                //    {
                //        #region 
                //        if (!monthlyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
                //        {
                //        }
                //        #endregion
                //    }
                //    monthlyReportWorkbook.Save();
                //}
                #endregion

                #region Create Early report

                //if (DateTime.Today.Day == 1 && DateTime.Today.Month == 1)
                //{
                //timestamp_begin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
                //timestamp_end = new DateTime(DateTime.Today.Year, 1, 1);

                //    filename = Helper.GetFileName(region.Name, ReportType.Year.ToString(), path, EnergyResource.Energy.ToString());

                //    WorkWithExcel YearReportWorkbook = new WorkWithExcel(filename);

                //    if (!YearReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                //    {

                //    }

                //    foreach (var branch in region.Branches)
                //    {
                //        #region 
                //        if (!YearReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
                //        {
                //        }
                //        #endregion
                //    }
                //    YearReportWorkbook.Save();
                //}
                #endregion



            }
            catch (Exception ex)
            {

                logger.Error(ex.Message);
                logger.Error(ex.Source);
            }
        }

        private static async Task SendReportToMailAsync(Region region)
        {
            string[]? attachedFile = null;

            string path = Helper.GetReportFolderByRegionName(_rootFolderName, region.Name);

            using (WorkWithMail mail = new WorkWithMail())
            {
                if (!mail.GetConfig())
                {

                }
                else
                {
                    List<MailingAddress> addres = Controler.GetListMailing(region.ID);

                    if (!Helper.GetAtachedFileName(ref attachedFile, path))
                    {
                    }


                    await mail.SendMailAsync(region.ID, region.Name, addres, attachedFile);
                }

            }

        }

    }
}
