using System.Drawing;
using SLPDBLibrary;
using SLPDBLibrary.Models;
using SLPHelper;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
using SLPReportCreater;
using OfficeOpenXml;
using System.Collections.Generic;
using OfficeOpenXml.Table.PivotTable;
using System.Text;

namespace SLPReportBuilder
{
    public static class ReportBuilderCore
    {
        private static Logger logger = LogManager.GetLogger("logger");
        private static List<SLPDBLibrary.Region> _regions = new List<SLPDBLibrary.Region>();

        private static int regionId;
        private static string regionName;
        private static string reportFolderName;
       
        

        public static void GenerateReport(string path)
        {
            reportFolderName = path;
            try
            {

                logger.Info("Get region list from database");
                var regions = Controler.GetRegion();

                if (regions != null)
                {
                    foreach (var item in regions)
                    {
                        logger.Info("An entry has been added to the list of regions: " + item.Name);
                        SLPDBLibrary.Region region = new SLPDBLibrary.Region(item.Id,item.Name);

                        logger.Info("Creating a list of branches for a region  : " + reportFolderName);
                        var branchList = Controler.GetBranchesInformation(item.Id);
                        foreach (var branch in branchList)
                        {
                            region.AddBranch(branch);
                        }

                        _regions.Add(region);

                    }
                }


                logger.Info("Create a thread for generating a power consumption report" + _regions[0].Name);
                Thread energyReportBuilder = new Thread(() => {
                    GenerateEnergyReport(_regions[0], reportFolderName);
                });
                

                logger.Info("Create a thread for generating a water consumption report" + _regions[0].Name);
                Thread waterReportBuilder = new Thread(() => {
                    GenerateWaterReport(_regions[0], reportFolderName);
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

                filename = Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Energy.ToString());

                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                if (!dailyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                {

                }

                foreach (var branch in region.Branches)
                {
                    #region 
                    if (!dailyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Day))
                    {
                    }

                    #endregion
                }
                dailyReportWorkbook.Save();

                #endregion

                #region Create Weekly report

                if(DateTime.Today.DayOfWeek == DayOfWeek.Monday) 
                {
                    filename = Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Energy.ToString());

                    WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                    if (!weeklyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                    {

                    }

                    foreach (var branch in region.Branches)
                    {
                        #region 
                        if (!weeklyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Week))
                        {
                        }
                        #endregion
                    }
                    weeklyReportWorkbook.Save();
                }
                #endregion

                #region Create Monthly report

                if (DateTime.Today.Day == 1)
                {
                    filename = Helper.GetFileName(region.Name, ReportType.Month.ToString(), path, EnergyResource.Energy.ToString());

                    WorkWithExcel monthlyReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                    if (!monthlyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                    {

                    }

                    foreach (var branch in region.Branches)
                    {
                        #region 
                        if (!monthlyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
                        {
                        }
                        #endregion
                    }
                    monthlyReportWorkbook.Save();
                }
                #endregion

                #region Create Early report

                if (DateTime.Today.Day == 1 && DateTime.Today.Month == 1)
                {
                    filename = Helper.GetFileName(region.Name, ReportType.Year.ToString(), path, EnergyResource.Energy.ToString());

                    WorkWithExcel YearReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                    if (!YearReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Energy))
                    {

                    }

                    foreach (var branch in region.Branches)
                    {
                        #region 
                        if (!YearReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
                        {
                        }
                        #endregion
                    }
                    YearReportWorkbook.Save();
                }
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

                filename = Helper.GetFileName(region.Name, ReportType.Day.ToString(), path, EnergyResource.Water.ToString());

                WorkWithExcel dailyReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                if (!dailyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Water))
                {

                }

                foreach (var branch in region.Branches)
                {
                    #region 
                    if (!dailyReportWorkbook.GenerateReportTemplateWater(branch, ReportType.Day))
                    {
                    }

                    #endregion
                }
                dailyReportWorkbook.Save();

                #endregion

                #region Create Weekly report

                if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                {
                    filename = Helper.GetFileName(region.Name, ReportType.Week.ToString(), path, EnergyResource.Water.ToString());

                    WorkWithExcel weeklyReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                    if (!weeklyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Water))
                    {

                    }

                    foreach (var branch in region.Branches)
                    {
                        #region 
                        if (!weeklyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Week))
                        {
                        }
                        #endregion
                    }
                    weeklyReportWorkbook.Save();
                }
                #endregion

                #region Create Monthly report

                if (DateTime.Today.Day == 1)
                {
                    filename = Helper.GetFileName(region.Name, ReportType.Month.ToString(), path, EnergyResource.Water.ToString());

                    WorkWithExcel monthlyReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                    if (!monthlyReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Water))
                    {

                    }

                    foreach (var branch in region.Branches)
                    {
                        #region 
                        if (!monthlyReportWorkbook.GenerateReportTemplateEnergy(branch, ReportType.Month))
                        {
                        }
                        #endregion
                    }
                    monthlyReportWorkbook.Save();
                }
                #endregion

                #region Create Early report

                if (DateTime.Today.Day == 1 && DateTime.Today.Month == 1)
                {
                    filename = Helper.GetFileName(region.Name, ReportType.Year.ToString(), path, EnergyResource.Water.ToString());

                    WorkWithExcel YearReportWorkbook = new WorkWithExcel(region.ID, region.Name, filename);

                    if (!YearReportWorkbook.GenerateBranchListWorksheet(region.Branches, EnergyResource.Water))
                    {

                    }

                    foreach (var branch in region.Branches)
                    {
                        #region 
                        if (!YearReportWorkbook.GenerateReportTemplateWater(branch, ReportType.Month))
                        {
                        }
                        #endregion
                    }
                    YearReportWorkbook.Save();
                }
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
