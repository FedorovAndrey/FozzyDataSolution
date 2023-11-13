using System.Text;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SLPDBLibrary;
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
        ExcelPackage excel;

        public WorkWithExcel(int regionId, string regionName, string reportFolderName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.regionId = regionId;
            this.regionName = regionName;
            this.reportFolderName = reportFolderName;
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
        private void GenerateReport(ReportType reportType, string reportFolderName)
        {
            logger.Info("Generating reports for the region : " + reportFolderName);

            switch (reportType)
            {
                case ReportType.Day:
                    logger.Info("Generation of daily consumption reports");

                    dateTime_Begin = DateTime.Today.AddDays(-1);
                    dateTime_End = DateTime.Today;
                    break;
                case ReportType.Week:
                    logger.Info("Generation of weekly consumption reports");

                    dateTime_Begin = DateTime.Today.AddDays(-7).AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                    dateTime_End = DateTime.Today.AddDays((-1) * (int)(DateTime.Today.DayOfWeek - 1));
                    break;
                case ReportType.Month:
                    logger.Info("Generation of monthly consumption reports");

                    dateTime_Begin = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 1);
                    dateTime_End = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case ReportType.Year:
                    logger.Info("Generation of annual consumption reports");

                    dateTime_Begin = new DateTime(DateTime.Today.AddYears(-1).Year, 1, 1);
                    dateTime_End = new DateTime(DateTime.Today.Year, 1, 1);
                    break;

            }

            #region Create excel Workbook
            logger.Info("Creating a report file for a region  : " + reportFolderName);

            try
            {
                FileInfo fileInfo = new FileInfo(Helper.GetFileName(regionName, reportType.ToString(), reportFolderName));
                excel = new ExcelPackage(fileInfo);

                logger.Info("Creating a list of branches for a region  : " + reportFolderName);

                List<BranchInformation> branches = Controler.GetBranchesInformation(regionId);

                logger.Info("The list of branches for reporting contains  : " + branches.Count + " items.");
                if (branches != null && branches.Count > 0)
                {
                    logger.Info("Creating a worksheet with a list of branches");
                    if (!GenerateBranchListWorksheet(ref excel, branches))
                    {
                        throw new Exception("Error creating branch list worksheet");
                    }

                    foreach (BranchInformation item in branches)
                    {
                        logger.Info("Creating a report template for a branch : " + item.Address);
                        ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(item.id.ToString());

                        if (!GenerateBranchReportTemplate(ref worksheet, item, reportType))
                        {
                            string errMessage = String.Concat("Generating a report template for the branch ", item.Address, " completed with an error.");
                            throw new Exception(errMessage);

                        }

                        logger.Info("Call of the function of filling with report data for a branch  : " + item.Address);
                        if (!FillBranchReportOfData(ref worksheet, item, reportType))
                        {
                            throw new Exception("Error receiving data for report generation");
                        }



                    }

                }

                excel.Save();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
            }

            #endregion

        }
        private bool GenerateBranchListWorksheet(ref ExcelPackage excel, List<BranchInformation> branches)
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

                    using (ExcelRange range = worksheet.Cells[String.Concat("D", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#";
                        range.SetCellValue(0, 0, item.meterCount);
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
                List<object> dataList = new List<object>();
                logger.Info("Getting the list of metering units for a branch : " + branch.Address);

                if (!Controler.GetMeterReport(ref branch, this.dateTime_Begin, this.dateTime_End))
                {
                    throw new Exception("Failed to retrieve data for the report  : " + branch.Address);
                }

                #region Fill data to worksheet


                if(branch.Meters.Count <= 0)
                {
                    logger.Warn(String.Concat("There is no list of metering units for branch: ", branch.Address));
                    return true;

                }

                for (int count = 0; count < branch.meterCount; count++)
                {
                    int startRegionRow = 6;
                    int startRegionColumn = 10;

                    if (branch.Meters[count]._data.Count <= 0)
                    {
                        logger.Warn(String.Concat("There is no parameter list for the metering unit: ", branch.Meters[count].MarkingPosition));
                        continue;
                    }

                    #region Filling with active energy import data

                    int index_Parametr = 0;
                    for (int i = 0; i < branch.Meters[count]._data.Count; i++)
                    {
                        if (branch.Meters[count]._data[i].Source.Contains("імпорт активної"))
                        {
                            index_Parametr = i;
                        }
                    }

                    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[1].Value;

                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[branch.Meters[count]._data[index_Parametr].values.Length - 1].Value;

                    }

                    startRegionRow = 15;
                    startRegionColumn = 6;

                    for (int i = 0; i < branch.Meters[count]._data[index_Parametr].values.Length - 1; i++)
                    {
                        double consumption = (double)(branch.Meters[count]._data[index_Parametr].values[i + 1].Value - branch.Meters[count]._data[index_Parametr].values[i].Value);

                        using (ExcelRange range = worksheet.Cells[startRegionRow + i, (4 * count) + startRegionColumn])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "#,##0.00";
                            range.Value = consumption;
                        }
                    }

                    #endregion

                    #region Filling with reactive energy import data

                    startRegionRow = 6;
                    startRegionColumn = 12;

                    for (int i = 0; i < branch.Meters[count]._data.Count; i++)
                    {
                        if (branch.Meters[count]._data[i].Source.Contains("імпорт реактивної"))
                        {
                            index_Parametr = i;
                        }
                    }


                    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[1].Value;

                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[branch.Meters[count]._data[index_Parametr].values.Length - 1].Value;

                    }

                    startRegionRow = 15;
                    startRegionColumn = 8;

                    for (int i = 0; i < branch.Meters[count]._data[index_Parametr].values.Length - 1; i++)
                    {
                        double consumption = (double)(branch.Meters[count]._data[index_Parametr].values[i + 1].Value - branch.Meters[count]._data[index_Parametr].values[i].Value);

                        using (ExcelRange range = worksheet.Cells[startRegionRow + i, (4 * count) + startRegionColumn])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "#,##0.00";
                            range.Value = consumption;
                        }
                    }

                    #endregion

                    #region Filling with active energy export data

                    startRegionRow = 6;
                    startRegionColumn = 11;

                    for (int i = 0; i < branch.Meters[count]._data.Count; i++)
                    {
                        if (branch.Meters[count]._data[i].Source.Contains("експорт активної"))
                        {
                            index_Parametr = i;
                        }
                    }


                    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[1].Value;

                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[branch.Meters[count]._data[index_Parametr].values.Length - 1].Value;

                    }

                    startRegionRow = 15;
                    startRegionColumn = 7;

                    for (int i = 0; i < branch.Meters[count]._data[index_Parametr].values.Length - 1; i++)
                    {
                        double consumption = (double)(branch.Meters[count]._data[index_Parametr].values[i + 1].Value - branch.Meters[count]._data[index_Parametr].values[i].Value);

                        using (ExcelRange range = worksheet.Cells[startRegionRow + i, (4 * count) + startRegionColumn])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "#,##0.00";
                            range.Value = consumption;
                        }
                    }

                    #endregion

                    #region Filling with reactive energy export data

                    startRegionRow = 6;
                    startRegionColumn = 13;

                    for (int i = 0; i < branch.Meters[count]._data.Count; i++)
                    {
                        if (branch.Meters[count]._data[i].Source.Contains("експорт реактивної"))
                        {
                            index_Parametr = i;
                        }
                    }


                    using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[1].Value;

                    }
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * count) + startRegionColumn])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "#,##0.00";
                        range.Value = branch.Meters[count]._data[index_Parametr].values[branch.Meters[count]._data[index_Parametr].values.Length - 1].Value;

                    }

                    startRegionRow = 15;
                    startRegionColumn = 9;

                    for (int i = 0; i < branch.Meters[count]._data[index_Parametr].values.Length - 1; i++)
                    {
                        double consumption = (double)(branch.Meters[count]._data[index_Parametr].values[i + 1].Value - branch.Meters[count]._data[index_Parametr].values[i].Value);

                        using (ExcelRange range = worksheet.Cells[startRegionRow + i, (4 * count) + startRegionColumn])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "#,##0.00";
                            range.Value = consumption;
                        }
                    }

                    #endregion

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
    }

}