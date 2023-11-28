using System;
using System.Diagnostics.Metrics;
using System.Text;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SLPDBLibrary;
using SLPDBLibrary.Models;
using SLPHelper;
using SLPMailSender;

namespace SLPReportCreater
{
    public class WorkWithExcel
    {
        private int regionId;
        private string regionName;
        private string reportFolderName;

        private DateTime dateTime_Begin;
        private DateTime dateTime_End;

        //Microsoft.Extensions.Logging.ILogger logger ;
        private Logger logger = LogManager.GetLogger("logger");
        private ExcelPackage excel;
        private string _filename = "";

        public ExcelPackage Excel { get => excel; set => excel = value; }

        public WorkWithExcel(int regionId, string regionName, string filename)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            this.regionId = regionId;
            this.regionName = regionName;

            _filename = filename;

            FileInfo fileInfo = new FileInfo(filename); 

            excel = new ExcelPackage(fileInfo);
        }

        public async void Generate()
        {
            string[] atachedFileName = { "" };

            logger.Info("Report generation for the region ID : " + regionId.ToString());
            try
            {
                StringBuilder sReportFolderByRegion = new StringBuilder(Helper.GetReportFolderByRegionName(reportFolderName, regionName));
                sReportFolderByRegion.Append(@"\");

                #region report generation unit

                GenerateReport(ReportType.Day, sReportFolderByRegion.ToString());

                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                {

                    GenerateReport(ReportType.Week, sReportFolderByRegion.ToString());
                }
                if (DateTime.Now.Day == 1)
                {

                    GenerateReport(ReportType.Month, sReportFolderByRegion.ToString());
                }
                if (DateTime.Now.DayOfYear == 1)
                {

                    GenerateReport(ReportType.Year, sReportFolderByRegion.ToString());
                }
                #endregion

                #region Preparing a list of addresses for mailing reports
                logger.Info("Creating a mailing list for reports");

                List<MailingAddress> mailsAdress = Controler.GetListMailing(regionId);

                if (mailsAdress != null && mailsAdress.Count > 0)
                {
                    using (WorkWithMail mails = new WorkWithMail())
                    {
                        logger.Info("Obtaining mail server configuration");
                        mails.GetConfig();

                        #region Preparing a list of files to be sent

                        logger.Info("Preparing a list of files to be sent");
                        if (Directory.Exists(sReportFolderByRegion.ToString()))
                        {
                            atachedFileName = Directory.GetFiles(sReportFolderByRegion.ToString());
                        }
                        #endregion

                        logger.Info("Call of asynchronous method for sending reports");

                        await mails.SendMailAsync(regionId, regionName, mailsAdress, atachedFileName);

                        logger.Info("Clearing the Reporting Documents Storage Folder");
                        if (!Helper.ClearReportFolder(sReportFolderByRegion.ToString()))
                        {
                            logger.Error("Clearing the reports folder ended with an error");
                        }

                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("End report generation for the region ID" + regionId.ToString());
        }

        public bool Save()
        {
            bool bResult = false;

            try
            {
                excel.Save();

                bResult = true;
            }
            catch {  }   


            return bResult;
        }
        private void GenerateReport(ReportType reportType, string reportFolderName)
        {
            //logger.Info("Generating reports for the region : " + reportFolderName);

            //switch (reportType)
            //{
            //    case ReportType.Day:
            //        logger.Info("Generation of daily consumption reports");

            //        dateTime_Begin = DateTime.Today.AddDays(-1);
            //        dateTime_End = DateTime.Today;
            //        break;
            //    case ReportType.Week:
            //        logger.Info("Generation of weekly consumption reports");

            //        dateTime_Begin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
            //        dateTime_End = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
            //        break;
            //    case ReportType.Month:
            //        logger.Info("Generation of monthly consumption reports");

            //        dateTime_Begin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
            //        dateTime_End = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //        break;
            //    case ReportType.Year:
            //        logger.Info("Generation of annual consumption reports");

            //        dateTime_Begin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
            //        dateTime_End = new DateTime(DateTime.Today.Year, 1, 1);
            //        break;

            //}

            //#region Create excel Workbook
            //logger.Info("Creating a report file for a region  : " + reportFolderName);

            //try
            //{
            //    FileInfo fileInfo = new FileInfo(Helper.GetFileName(regionName, reportType.ToString(), reportFolderName));
            //    excel = new ExcelPackage(fileInfo);

            //    logger.Info("Creating a list of branches for a region  : " + reportFolderName);

            //    List<BranchInformation> branches = Controler.GetBranchesInformation(regionId);

            //    logger.Info("The list of branches for reporting contains  : " + branches.Count + " items.");
            //    if (branches != null && branches.Count > 0)
            //    {
            //        logger.Info("Creating a worksheet with a list of branches");
            //        if (!GenerateBranchListWorksheet(ref excel, branches))
            //        {
            //            throw new Exception("Error creating branch list worksheet");
            //        }

            //        foreach (BranchInformation item in branches)
            //        {
            //            logger.Info("Creating a report template for a branch : " + item.Address);
            //            ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(item.id.ToString());

            //            if (!GenerateBranchReportTemplate(ref worksheet, item, reportType))
            //            {
            //                string errMessage = String.Concat("Generating a report template for the branch ", item.Address, " completed with an error.");
            //                throw new Exception(errMessage);

            //            }

            //            logger.Info("Call of the function of filling with report data for a branch  : " + item.Address);
            //            if (!FillBranchReportOfData(ref worksheet, item, reportType))
            //            {
            //                throw new Exception("Error receiving data for report generation");
            //            }

            //            if (!FillBranchTotalValue(ref worksheet, item, reportType))
            //            {
                            
            //            }



            //        }

            //    }

            //    excel.Save();
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex.Message);
            //}
            //finally
            //{
            //}



            //#endregion

        }
        
        private bool GenerateBranchReportTemplate(ref ExcelWorksheet worksheet, BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            DateTime dateTime = DateTime.Now;
            string reportTitle = "";
            try
            {
                switch (reportType)
                {
                    case ReportType.Day:
                        reportTitle = String.Concat("Добовий графік спожитої електроенергії за: ", dateTime_Begin.Date.ToShortDateString());
                        break;
                    case ReportType.Week:
                    case ReportType.Month:
                    case ReportType.Year:
                        reportTitle = String.Concat("Графік спожитої електроенергії з: ", dateTime_Begin.Date.ToShortDateString(), " по ", dateTime_End.Date.ToShortDateString());
                        break;
                    default:
                        reportTitle = "Графік спожитої електроенергії";
                        break;
                }

                //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(branch.id.ToString());

                using (ExcelRange range = worksheet.Cells["A2:D9"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 12, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat(branch.City, ", ", branch.Address));
                }

                using (ExcelRange range = worksheet.Cells["F6:I6"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_Begin.Date.ToShortDateString()));
                }

                using (ExcelRange range = worksheet.Cells["F7:I7"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_End.Date.ToShortDateString()));
                }

                using (ExcelRange range = worksheet.Cells["A11:B11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Italic = true;
                    range.SetCellValue(0, 0, "Час створення звіту:");
                }

                using (ExcelRange range = worksheet.Cells["C11:D11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Italic = true;
                    range.SetCellValue(0, 0, dateTime.Date.ToShortDateString() + " " + dateTime.ToShortTimeString());
                }

                using (ExcelRange range = worksheet.Cells["F11:L11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, reportTitle);
                }

                using (ExcelRange range = worksheet.Cells["A13:A14"])
                {
                    range.Merge = true;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                using (ExcelRange range = worksheet.Cells["B13:E13"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, "Разом");
                }

                using (ExcelRange range = worksheet.Cells["B14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "A+");
                }

                using (ExcelRange range = worksheet.Cells["C14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "A-");
                }

                using (ExcelRange range = worksheet.Cells["D14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "P+");
                }

                using (ExcelRange range = worksheet.Cells["E14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "P-");
                }

                using (ExcelRange range = worksheet.Cells["A13:E14"])
                {
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                #region Creating table structures in a template

                var meters = Controler.GetMeterByBranchId(branch.id);
                if (meters != null && meters.Count > 0)
                {
                    int startRegionRow = 4;
                    int startRegionColumn = 10;

                    for (int i = 0; i < meters.Count; i++)
                    {
                        using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn, startRegionRow, startRegionColumn + 1])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, meters[i].Legend);
                        }
                        using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn + 2, startRegionRow, startRegionColumn + 3])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, meters[i].SerialNumber);
                        }

                        using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, "A+");
                        }
                        using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn + 1])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, "A-");
                        }
                        using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn + 2])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, "P+");
                        }
                        using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn + 3])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, "P-");
                        }

                        startRegionColumn = startRegionColumn + 4;
                    }

                    using (ExcelRange range = worksheet.Cells[4, 10, 5, 10 + meters.Count * 4])
                    {
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    }

                    worksheet.Cells[4, 10, 5, 10 + meters.Count * 4].Copy(worksheet.Cells["F13"]);

                }
                #endregion

                bResult = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return bResult;
        }
        private bool FillBranchReportOfData(ref ExcelWorksheet worksheet, BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            try
            {
                //logger.Info("Getting the list of metering units for a branch : " + branch.Address);

                //if (!Controler.GetMeterReport(ref branch, this.dateTime_Begin, this.dateTime_End))
                //{
                //    throw new Exception("Failed to retrieve data for the report  : " + branch.Address);
                //}

                //#region Fill data to worksheet

                //if(branch.Meters.Count <= 0)
                //{
                //    logger.Warn(String.Concat("There is no list of metering units for branch: ", branch.Address));
                //    return true;

                //}
                
                //for (int m_index = 0; m_index < branch.meterCount; m_index++)
                //{
                //    int startRegionRow = 6;
                //    int startRegionColumn = 10;

                //    if (branch.Meters[m_index]._data.Count <= 0)
                //    {
                //        logger.Warn(String.Concat("There is no parameter list for the metering unit: ", branch.Meters[m_index].MarkingPosition));
                //        continue;
                //    }
                    
                //    #region Filling with active energy import data

                //    int index_Parametr = 0;
                //    for (int i = 0; i < branch.Meters[m_index]._data.Count; i++)
                //    {
                //        if (branch.Meters[m_index]._data[i].Source.Contains("імпорт активної"))
                //        {
                //            index_Parametr = i;
                //        }
                //    }

                //    if (branch.Meters[m_index]._data[index_Parametr].values.Length <= 0)
                //    {
                //        logger.Warn(String.Concat("No data available for the metering station: ", branch.Meters[m_index].MarkingPosition));
                //        continue;
                //    }
                    
                //    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[0].Value;

                //    }
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[branch.Meters[m_index]._data[index_Parametr].values.Length - 1].Value;

                //    }

                //    startRegionRow = 15;
                //    startRegionColumn = 6;

                //    for (int i = 0; i < branch.Meters[m_index]._data[index_Parametr].values.Length - 1; i++)
                //    {
                //        double value_next = (double)branch.Meters[m_index]._data[index_Parametr].values[i + 1].Value;
                //        double value = (double)branch.Meters[m_index]._data[index_Parametr].values[i].Value;
                //        double consumption = value_next - value;

                //        DateTime dt_begin = branch.Meters[m_index]._data[index_Parametr].values[i].Timestamp;
                //        DateTime dt_end = branch.Meters[m_index]._data[index_Parametr].values[i + 1].Timestamp;
                //        string date_interval = String.Concat( dt_begin.ToShortTimeString(), " - ", dt_end.ToShortTimeString());

                //        using (ExcelRange range = worksheet.Cells[startRegionRow + i, (4 * m_index) + startRegionColumn])
                //        {
                //            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //            range.Style.Numberformat.Format = "#,##0.00";
                //            range.Value = consumption;
                //        }

                //        using (ExcelRange range = worksheet.Cells[startRegionRow + i, 1])
                //        {
                //            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            
                //            range.Value = date_interval;
                //        }


                //    }

                //    #endregion

                //    #region Filling with reactive energy import data

                //    startRegionRow = 6;
                //    startRegionColumn = 12;

                //    for (int i = 0; i < branch.Meters[m_index]._data.Count; i++)
                //    {
                //        if (branch.Meters[m_index]._data[i].Source.Contains("імпорт реактивної"))
                //        {
                //            index_Parametr = i;
                //        }
                //    }

                //    if (branch.Meters[m_index]._data[index_Parametr].values.Length <= 0)
                //    {
                //        logger.Warn(String.Concat("No data available for the metering station: ", branch.Meters[m_index].MarkingPosition));
                //        continue;
                //    }

                //    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[0].Value;

                //    }
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[branch.Meters[m_index]._data[index_Parametr].values.Length - 1].Value;

                //    }

                //    startRegionRow = 15;
                //    startRegionColumn = 8;

                //    for (int i = 0; i < branch.Meters[m_index]._data[index_Parametr].values.Length - 1; i++)
                //    {
                //        double consumption = (double)(branch.Meters[m_index]._data[index_Parametr].values[i + 1].Value - branch.Meters[m_index]._data[index_Parametr].values[i].Value);

                //        using (ExcelRange range = worksheet.Cells[startRegionRow + i, (4 * m_index) + startRegionColumn])
                //        {
                //            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //            range.Style.Numberformat.Format = "#,##0.00";
                //            range.Value = consumption;
                //        }
                //    }

                //    #endregion

                //    #region Filling with active energy export data

                //    startRegionRow = 6;
                //    startRegionColumn = 11;

                //    for (int i = 0; i < branch.Meters[m_index]._data.Count; i++)
                //    {
                //        if (branch.Meters[m_index]._data[i].Source.Contains("експорт активної"))
                //        {
                //            index_Parametr = i;
                //        }
                //    }

                //    if (branch.Meters[m_index]._data[index_Parametr].values.Length <= 0)
                //    {
                //        logger.Warn(String.Concat("No data available for the metering station: ", branch.Meters[m_index].MarkingPosition));
                //        continue;
                //    }

                //    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[0].Value;

                //    }
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[branch.Meters[m_index]._data[index_Parametr].values.Length - 1].Value;

                //    }

                //    startRegionRow = 15;
                //    startRegionColumn = 7;

                //    for (int val_index = 0; val_index < branch.Meters[m_index]._data[index_Parametr].values.Length -1; val_index++)
                //    {
                //        double value_next = (double)branch.Meters[m_index]._data[index_Parametr].values[val_index + 1].Value;
                //        double value = (double)branch.Meters[m_index]._data[index_Parametr].values[val_index].Value;
                //        double consumption = value_next - value;

                //        using (ExcelRange range = worksheet.Cells[startRegionRow + val_index, (4 * m_index) + startRegionColumn])
                //        {
                //            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //            range.Style.Numberformat.Format = "#,##0.00";
                //            range.Value = consumption;
                //        }
                //    }

                //    #endregion

                //    #region Filling with reactive energy export data

                //    startRegionRow = 6;
                //    startRegionColumn = 13;

                //    for (int i = 0; i < branch.Meters[m_index]._data.Count; i++)
                //    {
                //        if (branch.Meters[m_index]._data[i].Source.Contains("експорт реактивної"))
                //        {
                //            index_Parametr = i;
                //        }
                //    }

                //    if (branch.Meters[m_index]._data[index_Parametr].values.Length <= 0)
                //    {
                //        logger.Warn(String.Concat("No data available for the metering station: ", branch.Meters[m_index].MarkingPosition));
                //        continue;
                //    }

                //    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[0].Value;

                //    }
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * m_index) + startRegionColumn])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.Value = branch.Meters[m_index]._data[index_Parametr].values[branch.Meters[m_index]._data[index_Parametr].values.Length - 1].Value;

                //    }

                //    startRegionRow = 15;
                //    startRegionColumn = 9;

                //    for (int i = 0; i < branch.Meters[m_index]._data[index_Parametr].values.Length - 1 ; i++)
                //    {
                //        double consumption = (double)(branch.Meters[m_index]._data[index_Parametr].values[i + 1].Value - branch.Meters[m_index]._data[index_Parametr].values[i].Value);

                //        using (ExcelRange range = worksheet.Cells[startRegionRow + i, (4 * m_index) + startRegionColumn])
                //        {
                //            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //            range.Style.Numberformat.Format = "#,##0.00";
                //            range.Value = consumption;
                //        }

                        
                //    }

                //    #endregion
                    

                //}
                //#endregion




                bResult = true;
            }
            catch (Exception ex)
            {
                logger.Error(String.Concat(ex.Source , " - " , ex.Message));
                logger.Error(ex.InnerException);
            }
            return bResult;

        }

        private bool FillBranchTotalValue(ref ExcelWorksheet worksheet, BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            try {
                //int data_count = 0;
                //int startRegionRow = 15;
                //int startRegionColumn = 1;

                //StringBuilder s_formula = new StringBuilder("=(");
                //for (int k = 0; k < branch.Meters.Count; k++)
                //{
                //    int index = 4 + (k * 4);
                //    s_formula.Append("RC[");
                //    s_formula.Append(index.ToString());
                //    s_formula.Append("]");

                //    if (k < (branch.Meters.Count - 1))
                //    {
                //        s_formula.Append(" + ");
                //    }

                //}
                //s_formula.Append(')');


                //switch (reportType)
                //{
                //    case ReportType.Day:
                //        data_count = 24;
                //        break;
                //    case ReportType.Week:
                //        data_count =7;
                //        break;
                //    case ReportType.Month:
                //        data_count = 31;
                //        break;
                //    case ReportType.Year:
                //        data_count = 12;
                //        break;


                //}

                //for (int i = 0; i < data_count; i++)
                //{
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + i, 2])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.FormulaR1C1 = s_formula.ToString();
                //    }
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + i, 3])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.FormulaR1C1 = s_formula.ToString();
                //    }
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + i, 4])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.FormulaR1C1 = s_formula.ToString();
                //    }
                //    using (ExcelRange range = worksheet.Cells[startRegionRow + i, 5])
                //    {
                //        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                //        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                //        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //        range.Style.Numberformat.Format = "#,##0.00";
                //        range.FormulaR1C1 = s_formula.ToString();
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message); 
                logger.Error(ex.InnerException);
            }

            return bResult;

        }

        public bool GenerateBranchListWorksheet(List<BranchInformation> branches, EnergyResource resource)
        {
            bool bResult = false;

            try
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Branch List");

                using (ExcelRange range = worksheet.Cells["A1:A2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Філія");
                }

                using (ExcelRange range = worksheet.Cells["B1:B2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Місто");
                }

                using (ExcelRange range = worksheet.Cells["C1:C2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Адреса");
                }

                using (ExcelRange range = worksheet.Cells["D1:D2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Кількість лічильників");
                }
                using (ExcelRange range = worksheet.Cells["E1:E2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Споживання кВт*год");
                }
                using (ExcelRange range = worksheet.Cells["F1:F2"])
                {
                    range.Merge = true;
                    range.AutoFitColumns();
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    range.SetCellValue(0, 0, "Посилання");
                }

                int startRow = 3;

                foreach (BranchInformation item in branches)
                {
                    using (ExcelRange range = worksheet.Cells[String.Concat("A", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, item.id.ToString());
                    }
                    using (ExcelRange range = worksheet.Cells[String.Concat("B", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, item.City);
                    }
                    using (ExcelRange range = worksheet.Cells[String.Concat("C", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, item.Address);
                    }

                    if (resource == EnergyResource.Energy)
                    {
                        using (ExcelRange range = worksheet.Cells[String.Concat("D", startRow)])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "#";
                            range.SetCellValue(0, 0, item.EnergyMeters.Count);
                        }
                    }

                    if (resource == EnergyResource.Water)
                    {
                        using (ExcelRange range = worksheet.Cells[String.Concat("D", startRow)])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "#";
                            range.SetCellValue(0, 0, item.WaterMeters.Count);
                        }
                    }

                    using (ExcelRange range = worksheet.Cells[String.Concat("E", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.SetCellValue(0, 0, "0.00");
                    }

                    using (ExcelRange range = worksheet.Cells[String.Concat("F", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.SetHyperlink(new ExcelHyperLink(String.Concat(item.id, "!A1"), "Перейти"));
                    }
                    startRow++;
                }
                bResult = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return bResult;
        }

        public bool GenerateReportTemplateEnergy(BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            DateTime dateTime = DateTime.Now;
            string reportTitle = String.Concat("Графік спожитої електроенергії за: ", dateTime_Begin.Date.ToShortDateString());

            try
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(branch.id.ToString());

                #region Formation of report template header

                /* Field address */
                using (ExcelRange range = worksheet.Cells["A2:D9"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 12, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat(branch.City, ", ", branch.Address));
                }

                /* Field consumption rates at the beginning of the reporting period */
                using (ExcelRange range = worksheet.Cells["F6:I6"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_Begin.Date.ToShortDateString()));
                }

                /* Field consumption rates at the end of the reporting period */
                using (ExcelRange range = worksheet.Cells["F7:I7"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_End.Date.ToShortDateString()));
                }
                
                /* Field report generation time */
                using (ExcelRange range = worksheet.Cells["A11:B11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Italic = true;
                    range.SetCellValue(0, 0, "Час створення звіту:");
                }
                using (ExcelRange range = worksheet.Cells["C11:D11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Italic = true;
                    range.SetCellValue(0, 0, dateTime.Date.ToShortDateString() + " " + dateTime.ToShortTimeString());
                }

                /* Field name of the report */
                using (ExcelRange range = worksheet.Cells["F11:L11"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, reportTitle);
                }

                using (ExcelRange range = worksheet.Cells["A13:A14"])
                {
                    range.Merge = true;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }
                #endregion

                #region Formating of report template table for data

                #region Formating table of total value header
                
                /* Field Data and time header */
                using (ExcelRange range = worksheet.Cells["A13:A14"])
                {
                    range.Merge = true;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, "Дата");
                }
                /* Fields total value header */
                using (ExcelRange range = worksheet.Cells["B13:E13"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, "Разом");
                }

                /* Fields total value header (A+) */
                using (ExcelRange range = worksheet.Cells["B14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "A+");
                }

                /* Fields total value header (A-) */
                using (ExcelRange range = worksheet.Cells["C14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "A-");
                }

                /* Fields total value header (P+) */
                using (ExcelRange range = worksheet.Cells["D14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "P+");
                }

                /* Fields total value header (P-) */
                using (ExcelRange range = worksheet.Cells["E14"])
                {

                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.SetCellValue(0, 0, "P-");
                }

                //Create border Total fields
                using (ExcelRange range = worksheet.Cells["A13:E14"])
                {
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }
                #endregion

                #region Formating table header for meters

                int startRegionRow = 4;
                int startRegionColumn = 10;
                int countRow = 0;

                for (int i = 0; i < branch.EnergyMeters.Count; i++)
                {
                    using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn, startRegionRow, startRegionColumn + 1])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, branch.EnergyMeters[i].Legend);
                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn + 2, startRegionRow, startRegionColumn + 3])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, branch.EnergyMeters[i].SerialNumber);
                    }

                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, "A+");
                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn + 1])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, "A-");
                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn + 2])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, "P+");
                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn + 3])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, "P-");
                    }

                    startRegionColumn = startRegionColumn + 4;

                    
                }

                using (ExcelRange range = worksheet.Cells[4, 10, 5, 10 + branch.EnergyMeters.Count * 4])
                {
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                worksheet.Cells[4, 10, 5, 10 + branch.EnergyMeters.Count * 4].Copy(worksheet.Cells["F13"]);
                #endregion

                #region Formating table for data

                switch (reportType)
                {
                    case ReportType.Day: { countRow = 24; }; break;
                    case ReportType.Week: { countRow = 7; }; break;
                    case ReportType.Month: { countRow = 31; }; break;
                    case ReportType.Year: { countRow = 12; }; break;

                }

                string cellAddres = "";
                string formula = Helper.GetTotalFormulaRC(branch.EnergyMeters.Count);
                for (int i = 0; i < countRow; i++)
                {
                    /* Field total value A+ */
                    cellAddres = String.Concat("B", (15 + i).ToString());

                    using (ExcelRange range = worksheet.Cells[cellAddres]) 
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.FormulaR1C1 = (formula);
                    }

                    cellAddres = String.Concat("C", (15 + i).ToString());
                    using (ExcelRange range = worksheet.Cells[cellAddres])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.FormulaR1C1 = (formula);
                    }

                    cellAddres = String.Concat("D", (15 + i).ToString());
                    using (ExcelRange range = worksheet.Cells[cellAddres])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.FormulaR1C1 = (formula);
                    }

                    cellAddres = String.Concat("E", (15 + i).ToString());
                    using (ExcelRange range = worksheet.Cells[cellAddres])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.FormulaR1C1 = (formula);
                    }

                    for(int k = 0; k < branch.EnergyMeters.Count * 4; k++)
                    {
                        using (ExcelRange range = worksheet.Cells[(15 + i), 6 + k])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0,0, 0.00);
                        }
                    }
                }

                #endregion

                #endregion

                bResult = true;
            }
            catch(Exception ex) {

                logger.Error(ex.Source);
                logger.Error(ex.Message);
                
            }

            return bResult;
        }

        public bool GenerateReportTemplateWater(BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            DateTime dateTime = DateTime.Now;
            string reportTitle = String.Concat("Графік спожитої води за: ", dateTime_Begin.Date.ToShortDateString());
                try
                {
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(branch.id.ToString());

                    #region Formation of report template header

                    /* Field address */
                    using (ExcelRange range = worksheet.Cells["A2:D9"])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 12, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.SetCellValue(0, 0, String.Concat(branch.City, ", ", branch.Address));
                    }

                    /* Field consumption rates at the beginning of the reporting period */
                    using (ExcelRange range = worksheet.Cells["F6:I6"])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_Begin.Date.ToShortDateString()));
                    }

                    /* Field consumption rates at the end of the reporting period */
                    using (ExcelRange range = worksheet.Cells["F7:I7"])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", dateTime_End.Date.ToShortDateString()));
                    }

                    /* Field report generation time */
                    using (ExcelRange range = worksheet.Cells["A11:B11"])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Font.Bold = true;
                        range.Style.Font.Italic = true;
                        range.SetCellValue(0, 0, "Час створення звіту:");
                    }
                    using (ExcelRange range = worksheet.Cells["C11:D11"])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Font.Bold = true;
                        range.Style.Font.Italic = true;
                        range.SetCellValue(0, 0, dateTime.Date.ToShortDateString() + " " + dateTime.ToShortTimeString());
                    }

                    /* Field name of the report */
                    using (ExcelRange range = worksheet.Cells["F11:L11"])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, true, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.SetCellValue(0, 0, reportTitle);
                    }

                    using (ExcelRange range = worksheet.Cells["A13:A14"])
                    {
                        range.Merge = true;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    }
                    #endregion

                    #region Formating of report template table for data

                    #region Formating table of total value header

                    /* Field Data and time header */
                    using (ExcelRange range = worksheet.Cells["A13:A14"])
                    {
                        range.Merge = true;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.SetCellValue(0, 0, "Дата");
                    }
                    /* Fields total value header */
                    using (ExcelRange range = worksheet.Cells["B13"])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.SetCellValue(0, 0, "Разом");
                    }

                    /* Fields total value header (споживання) */
                    using (ExcelRange range = worksheet.Cells["B14"])
                    {

                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, "Споживання");
                    }

                    //Create border Total fields
                    using (ExcelRange range = worksheet.Cells["A13:A14"])
                    {
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    }
                    #endregion

                    #region Formating table header for meters

                    int startRegionRow = 4;
                    int startRegionColumn = 10;
                    int countRow = 0;

                    for (int i = 0; i < branch.WaterMeters.Count; i++)
                    {
                        using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn, startRegionRow, startRegionColumn + 1])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, branch.WaterMeters[i].Legend);
                        }
                        using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn + 2, startRegionRow, startRegionColumn + 3])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, branch.WaterMeters[i].SerialNumber);
                        }

                        using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.SetCellValue(0, 0, "Споживання");
                        }

                        startRegionColumn = startRegionColumn + 1;
                    }

                    using (ExcelRange range = worksheet.Cells[4, 10, 5, 10 + branch.EnergyMeters.Count * 1])
                    {
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    }

                    worksheet.Cells[4, 10, 5, 10 + branch.WaterMeters.Count * 1].Copy(worksheet.Cells["F13"]);
                    #endregion

                    #region Formating table for data

                    switch (reportType)
                    {
                        case ReportType.Day: { countRow = 24; }; break;
                        case ReportType.Week: { countRow = 7; }; break;
                        case ReportType.Month: { countRow = 31; }; break;
                        case ReportType.Year: { countRow = 12; }; break;

                    }

                    string cellAddres = "";
                    string formula = Helper.GetTotalFormulaRC(branch.WaterMeters.Count);
                    for (int i = 0; i < countRow; i++)
                    {
                        /* Field total value A+ */
                        cellAddres = String.Concat("B", (15 + i).ToString());

                        using (ExcelRange range = worksheet.Cells[cellAddres])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.FormulaR1C1 = (formula);
                        }

                        for (int k = 0; k < branch.WaterMeters.Count * 1; k++)
                        {
                            using (ExcelRange range = worksheet.Cells[(15 + i), 6 + k])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.SetCellValue(0, 0, 0.00);
                            }
                        }
                    }

                    #endregion

                    #endregion

                    bResult = true;
                }
            catch (Exception ex)
            {
                logger.Error(ex.Source);
                logger.Error(ex.Message);
            }
            return bResult;
        }
    }

}