using Microsoft.Extensions.Logging;
using NLog;
using SLPDBLibrary;
using SLPDBLibrary.Models;
using SLPHelper;
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

        public static void GenerateReport(string path)
        {
            _rootFolderName = path;
            try
            {
                logger.Info("Get region list from database");

                var regions = Controler.GetRegion();

                if (regions != null)
                {
                    foreach (var item in regions)
                    {
                        #region Creating folders for reports by region
                        if (!Helper.CreateFolderReportByRegions(_rootFolderName, item.Name))
                        { 
                        }
                        
                        #endregion
                        
                        logger.Info("An entry has been added to the list of regions: " + item.Name);
                        _regions.Add(new SLPDBLibrary.Region(item.Id, item.Name));
                    }
                }

                foreach (var region in _regions)
                {
                    logger.Info("Creating a list of branches for a region  : " + region.Name);

                    var branchList = Controler.GetBranchesInformation(region.ID);

                    if (branchList != null)
                    {
                        logger.Info(String.Concat("The regional ", region.Name, " includes ", branchList.Count.ToString(), " branches"));

                        foreach (var branch in branchList)
                        {
                            region.AddBranch(branch);
                        }
                    }
                    else { logger.Info(String.Concat("Region ", region.Name, "does not contain any branches")); }

                }

                _reportFolderByRegion = Path.Combine(_rootFolderName, _regions[0].Name);
                logger.Info("Create a thread for generating a power consumption report" + _regions[0].Name);
                Thread energyReportBuilder = new Thread(() =>
                {
                    GenerateEnergyReport(_regions[0], _reportFolderByRegion);
                });


                logger.Info("Create a thread for generating a water consumption report" + _regions[0].Name);
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

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        /*
         * 1. Создать файл Эксель для отчета
         * 2. Создать лист списка филиалов
         * 3. Создать цикл для прохода всех филиалов в регионе
         * 3.1 Сформировать отчет суточного потребления
         * 3.1.1 Создать Шаблон отчета для филиала 
         * 3.1.2 Получить данные потребления за период
         * 3.1.3 Заполнить данными шаблон отчета для филиала
         * 3.2 
         * 
         */
        private static void GenerateEnergyReport(SLPDBLibrary.Region region, string path)
        {

            logger.Info("Run of energy consumption report generation flow - " + region.Name);

            string filename = "";
            try
            {
                #region Create daily report

                region.TimestampBegin = DateTime.Today.AddDays(-1);
                region.TimestampEnd   = DateTime.Today;


                foreach (var branch in region.Branches)
                {
                    logger.Info(String.Concat("Obtaining data on daily electricity consumption for a branch office: ", branch.Address));

                    List<Meter> meters = branch.EnergyMeters;

                    if (!Controler.GetMeterData(ref meters, branch.ServerName, ReportType.Day, EnergyResource.Energy, region.TimestampBegin, region.TimestampEnd))
                    {
                        logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on daily electricity consumption failed!"));
                    }
                }

                logger.Info(String.Concat("Creating a report file for a region: ", region.Name));

                filename = Path.Combine(path,  Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Energy.ToString()));
                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, ReportType.Day, EnergyResource.Energy, region);

                dailyReportWorkbook.Generate();
                dailyReportWorkbook.Save();

                #endregion


                #region Create Weekly report

                //if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                //{
                //    timestamp_begin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                //    timestamp_end = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));

                //    filename = Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Energy.ToString());

                //    WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(filename);

                //    if (!weeklyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                //    {

                //    }

                //    foreach (var branch in region.Branches)
                //    {
                //        #region 
                //        if (!weeklyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Week))
                //        {
                //        }
                //        #endregion
                //    }
                //    weeklyReportWorkbook.Save();
                //}
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


                for (int i = 0; i < 2; i++)
                {
                    logger.Info(String.Concat("Obtaining data on daily water consumption for a branch office: ", region.Branches[i].Address));

                    List<Meter> meters = region.Branches[i].WaterMeters;

                    if (!Controler.GetMeterData(ref meters, region.Branches[i].ServerName, ReportType.Day, EnergyResource.Water, region.TimestampBegin, region.TimestampEnd))
                    {
                        logger.Warn(String.Concat(region.Branches[i].Address, " - The method of obtaining data on daily electricity consumption failed!"));
                    }
                }

                //foreach (var branch in region.Branches)
                //{
                //    logger.Info(String.Concat("Obtaining data on daily electricity consumption for a branch office: ", branch.Address));

                //    List<Meter> meters = branch.EnergyMeters;

                //    if (!Controler.GetMeterData(ref meters, branch.ServerName, ReportType.Day, EnergyResource.Energy))
                //    {
                //        logger.Warn(String.Concat(branch.Address, " - The method of obtaining data on daily electricity consumption failed!"));
                //    }
                //}

                logger.Info(String.Concat("Creating a report file for a region: ", region.Name));


                filename = Path.Combine(path, Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Energy.ToString()));


                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(filename, ReportType.Day, EnergyResource.Energy, region);

                dailyReportWorkbook.Generate();
                dailyReportWorkbook.Save();

                #endregion


                #region Create Weekly report

                //if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                //{
                //    timestamp_begin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                //    timestamp_end = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));

                //    filename = Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Energy.ToString());

                //    WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(filename);

                //    if (!weeklyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                //    {

                //    }

                //    foreach (var branch in region.Branches)
                //    {
                //        #region 
                //        if (!weeklyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Week))
                //        {
                //        }
                //        #endregion
                //    }
                //    weeklyReportWorkbook.Save();
                //}
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



    }
}
