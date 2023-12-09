using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Text;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table.PivotTable;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Ocsp;
using SLPDBLibrary;
using SLPHelper;
using SLPMailSender;
using static OfficeOpenXml.ExcelErrorValue;

namespace SLPReportCreater
{
    public class WorkWithExcel
    {
        private string          _sFolderReportName = "";
        private FileInfo        _fileInfo;
        private ReportType      _reportType;
        private EnergyResource  _resource;
        private SLPDBLibrary.Region _region;

        private Logger logger = LogManager.GetLogger("logger");
        private ExcelPackage _excel;
        private ExcelWorksheet _listOfBranchWorksheet;
        
        public WorkWithExcel(string filename, ReportType reportType, EnergyResource resource, SLPDBLibrary.Region region)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _reportType = reportType;
            _resource = resource;
            _region = region;

            _fileInfo = new FileInfo(filename);
            _excel = new ExcelPackage(_fileInfo);
            


        }

        public void Generate()
        {
            try
            {
                if (this._resource == EnergyResource.Energy) {
                    if (!GenerateEnergyReport()) 
                    { 
                    }
                };

                if(this._resource == EnergyResource.Water) 
                {
                    if (!GenerateWaterReport())
                    { 
                    }
                };
                            
            }
            catch (Exception ex)
            { 
                logger.Error(ex.Message);
                logger.Error(ex.Source);
            }
        }

        private bool GenerateEnergyReport()
        {
            bool bResult = false;
            try
            {
                GenerateBranchListWorksheet(_region.Branches, EnergyResource.Energy, _reportType);
                foreach (var branch in _region.Branches)
                {
                    logger.Info(String.Concat("Branch: ", branch.Address, " - Create electricity report sheet"));
                    if (branch.EnergyMeters.Count > 0)
                    {
                        ExcelWorksheet worksheet = _excel.Workbook.Worksheets.Add(branch.id.ToString());

                        GenerateReportTemplateEnergy(ref worksheet, branch, _reportType, _region.TimestampBegin, _region.TimestampEnd);

                        FillReportDataEnergy(ref worksheet, branch, _reportType);

                    }
                }
            }
            catch
            { 
            }
            return bResult;
        }
        private bool GenerateWaterReport()
        {
            bool bResult = false;

            try 
            {
                GenerateBranchListWorksheet(_region.Branches, EnergyResource.Water, _reportType);

                foreach (var branch in _region.Branches)
                {
                    logger.Info(String.Concat(branch.Address, " - Create water report sheet"));

                    if (branch.WaterMeters.Count > 0)
                    {
                        ExcelWorksheet worksheet = _excel.Workbook.Worksheets.Add(branch.id.ToString());

                        GenerateReportTemplateWater(ref worksheet, branch, _reportType, _region.TimestampBegin, _region.TimestampEnd);
                        FillReportDataWater(ref worksheet, branch, _reportType);
                    }
                    else
                    {
                        logger.Warn(String.Concat(branch.Address, " - There is no data on metering units in the data base"));
                    }

                }

                bResult = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.Source);
            }
            return bResult;
        }

        public bool Save()
        {
            bool bResult = false;

            try
            {
                
                _excel.Save();

                bResult = true;
            }
            catch { }


            return bResult;
        }
        private List<double> GetConsuptionValues(List<TrendValue> values)
        {
            List<double>lResult = new List<double>();
            double consumption = 0;

            for(int i = 0; i < values.Count - 1; i++)
            {
                consumption = values[i + 1].Value - values[i].Value;
                lResult.Add(consumption);
            }
            return lResult;

        }
        private List<string> GetDateTimeValues(List<TrendValue> values, ReportType reportType)
        {
            List<string> lResult = new List<string>();
            string interval = "";

            for (int i = 0; i < values.Count - 1; i++)
            {
                switch (reportType)
                {
                    case ReportType.Day:
                        interval = String.Concat(values[i].Timestamp.ToShortTimeString(), "-", values[i + 1].Timestamp.ToShortTimeString());
                        break;
                    case ReportType.Week: 
                    case ReportType.Month:
                    case ReportType.Year:
                        interval = String.Concat(values[i].Timestamp.ToShortDateString(), "-", values[i + 1].Timestamp.ToShortDateString());
                        break; 
                }
                
                lResult.Add(interval);
            }
            return lResult;

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
                        reportTitle = String.Concat("Добовий графік спожитої електроенергії за: ", "");
                        break;
                    case ReportType.Week:
                    case ReportType.Month:
                    case ReportType.Year:
                        reportTitle = String.Concat("Графік спожитої електроенергії з: ", "", " по ", "");
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
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", ""));
                }

                using (ExcelRange range = worksheet.Cells["F7:I7"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ",""));
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
                logger.Error(String.Concat(ex.Source, " - ", ex.Message));
                logger.Error(ex.InnerException);
            }
            return bResult;

        }

        private bool GenerateBranchListWorksheet(List<BranchInformation> branches, EnergyResource resource, ReportType reportType)
        {
            bool bResult = false;

            try
            {
                ExcelWorksheet worksheet = _excel.Workbook.Worksheets.Add("Branch List");
                
                int rowIndex = 0;

                switch (reportType)
                {
                    case ReportType.Day:
                        rowIndex = 39;
                        break;
                    case ReportType.Week:
                        rowIndex = 22;
                        break;
                    case ReportType.Month:
                        rowIndex = 46;
                        break;
                    case ReportType.Year:
                        rowIndex = 27;
                        break;
                }


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


                    string s_formula = Helper.GetLinkToTotalValue(item.id, rowIndex);

                    using (ExcelRange range = worksheet.Cells[String.Concat("E", startRow)])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "0.00";
                        range.Formula = s_formula;
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

        private bool GenerateReportTemplateEnergy(ref ExcelWorksheet worksheet, BranchInformation branch, ReportType reportType, DateTime timestampBegin, DateTime timestampEnd)
        {
            bool bResult = false;

            DateTime dateTime = DateTime.Now;
            string reportTitle = String.Concat("Графік спожитої електроенергії за: ", timestampBegin.ToShortDateString());

            try
            {
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
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", timestampBegin.ToString()));
                }

                /* Field consumption rates at the end of the reporting period */
                using (ExcelRange range = worksheet.Cells["F7:I7"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", timestampEnd.ToString()));
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
                        range.Style.Numberformat.Format = "0.00";
                        range.FormulaR1C1 = (formula);
                    }

                    cellAddres = String.Concat("C", (15 + i).ToString());
                    using (ExcelRange range = worksheet.Cells[cellAddres])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "0.00";
                        range.FormulaR1C1 = (formula);
                    }

                    cellAddres = String.Concat("D", (15 + i).ToString());
                    using (ExcelRange range = worksheet.Cells[cellAddres])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "0.00";
                        range.FormulaR1C1 = (formula);
                    }

                    cellAddres = String.Concat("E", (15 + i).ToString());
                    using (ExcelRange range = worksheet.Cells[cellAddres])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "0.00";
                        range.FormulaR1C1 = (formula);
                    }

                    for (int k = 0; k < branch.EnergyMeters.Count * 4; k++)
                    {
                        using (ExcelRange range = worksheet.Cells[(15 + i), 6 + k])
                        {
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "0.00";
                            range.SetCellValue(0, 0, 0.00);
                        }
                    }
                }

                startRegionColumn = 2;
                startRegionRow = 15;
                for (int k = 0; k < 4 + (branch.EnergyMeters.Count * 4); k++)
                {
                    string s_formuls = Helper.GetSumFormulaRC(countRow);

                    using (ExcelRange range = worksheet.Cells[startRegionRow + countRow, startRegionColumn + k])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "0.00";
                        range.FormulaR1C1 = (s_formuls);
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
        public bool GenerateReportTemplateWater(ref ExcelWorksheet worksheet, BranchInformation branch, ReportType reportType, DateTime timestampBegin, DateTime timestampEnd)
        {
            bool bResult = false;

            DateTime dateTime = DateTime.Now;
            string reportTitle = String.Concat("Графік спожитої води за: ", timestampBegin.ToShortDateString());

            try
            {
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
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", timestampBegin.ToString()));
                }

                /* Field consumption rates at the end of the reporting period */
                using (ExcelRange range = worksheet.Cells["F7:I7"])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, false, true, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.SetCellValue(0, 0, String.Concat("Покази лічильників на: ", timestampEnd.ToString()));
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
                using (ExcelRange range = worksheet.Cells["B13:B13"])
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
                    range.SetCellValue(0, 0, "Споживання");
                }

                //Create border Total fields
                using (ExcelRange range = worksheet.Cells["A13:B14"])
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
                    // Fill Legend meter
                    using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, branch.WaterMeters[i].Legend);
                    }

                    // Fill SerialNumber meter
                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, branch.WaterMeters[i].SerialNumber);
                    }

                    // Fill MarkingPosition meter
                    using (ExcelRange range = worksheet.Cells[startRegionRow, startRegionColumn + 1])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.SetCellValue(0, 0, branch.WaterMeters[i].MarkingPosition);
                    }



                    using (ExcelRange range = worksheet.Cells[startRegionRow + 1, startRegionColumn + 1])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        //range.SetCellValue(0, 0, "Споживання");
                    }
                    startRegionColumn = startRegionColumn + 2;


                }

                using (ExcelRange range = worksheet.Cells[4, 10, 5, 10 + (branch.WaterMeters.Count * 2) - 1])
                {
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                }

                worksheet.Cells[4, 10, 5, 10 + (branch.EnergyMeters.Count * 2) - 1].Copy(worksheet.Cells["C13"]);
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
                string formula = "";
                formula = Helper.GetWaterTotalFormulaRC(branch.WaterMeters.Count);

                for (int i = 0; i < countRow; i++)
                {
                    /* Field total value consumption */
                    cellAddres = String.Concat("B", (15 + i).ToString());
                    using (ExcelRange range = worksheet.Cells[cellAddres])
                    {
                        range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "0.00";
                        range.FormulaR1C1 = (formula);
                    }

                    for (int k = 0; k < branch.WaterMeters.Count; k++)
                    {
                        int startCol = 2 + (k * 2) + 1;
                        int endCol = 2 + (k * 2) + 2;
                        using (ExcelRange range = worksheet.Cells[(15 + i), startCol, (15 + i), endCol])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.Style.Numberformat.Format = "0.00";
                            range.SetCellValue(0, 0, 0.00);
                        }
                    }
                }

                startRegionColumn = 2;
                startRegionRow = 15;
                string s_formuls = Helper.GetSumFormulaRC(countRow);

                using (ExcelRange range = worksheet.Cells[startRegionRow + countRow, startRegionColumn])
                {
                    range.Merge = true;
                    range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.Style.Numberformat.Format = "0.00";
                    range.FormulaR1C1 = (s_formuls);
                }

                for (int k = 0; k < branch.WaterMeters.Count; k++)
                {
                    using (ExcelRange range = worksheet.Cells[startRegionRow + countRow, 1 + startRegionColumn + (k * 2), startRegionRow + countRow, 2 + startRegionColumn + (k * 2)])
                    {
                        range.Merge = true;
                        range.Style.Font.SetFromFont("Arial", 10, true, false, false, false);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.Numberformat.Format = "0.00";
                        range.FormulaR1C1 = (s_formuls);
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
        private bool FillReportDataEnergy(ref ExcelWorksheet worksheet, BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            try {
                logger.Info("Filling in the data on electricity consumption by the branch : " + branch.Address);

                for (int meter_index = 0; meter_index < branch.EnergyMeters.Count; meter_index++)
                {
                    if (branch.EnergyMeters[meter_index].Parametr.Count <= 0)
                    {
                        logger.Info("No parameters saved for metering unit Name : " + branch.EnergyMeters[meter_index].Legend);
                        continue;
                    };
                    
                    #region Fill Import active power

                    var result = (from apower in branch.EnergyMeters[meter_index].Parametr
                                  where (apower.Source.Contains("Загальний імпорт активної енергії"))
                                  select new MeterData
                                  {
                                      Source = apower.Source,
                                      Values = (
                                      from val in apower.Values
                                      select new TrendValue
                                      {
                                          Timestamp = val.Timestamp,
                                          Value = val.Value
                                      }).ToList()

                                  }).ToList<MeterData>();

                    if(result != null && result.Count > 0) 
                    {
                        int startRegionRow = 6;
                        int startRegionColumn = 10;

                        MeterData data = result[0];

                        if (data.Source != null && data.Values != null && data.Values.Count > 0)
                        {
                            List<double> values = GetConsuptionValues(data.Values);
                            List<string> timestamps = GetDateTimeValues(data.Values, reportType);

                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.FirstOrDefault().Value;

                            }

                            using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.Last().Value;

                            }

                            startRegionRow = 15;
                            startRegionColumn = 6;

                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.LoadFromCollection(values);

                            }

                            for (int row_index = 0; row_index < timestamps.Count; row_index++)
                            {
                                using (ExcelRange range = worksheet.Cells[startRegionRow + row_index, 1])
                                {
                                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    range.Style.Numberformat.Format = "0.00";
                                    range.SetCellValue(0, 0, timestamps[row_index]);
                                }
                            }

                        }
                        else
                        {
                            logger.Warn(String.Concat( branch.Address, ":",
                                branch.EnergyMeters[meter_index].MarkingPosition, " - ",
                                data.Source, " - Parameter values are missing in the database"));
                        }

                    }
                    #endregion

                    #region Fill Export active power

                    result = (from apower in branch.EnergyMeters[meter_index].Parametr
                                  where (apower.Source.Contains("Загальний експорт активної енергії"))
                                  select new MeterData
                                  {
                                      Source = apower.Source,
                                      Values = (
                                      from val in apower.Values
                                      select new TrendValue
                                      {
                                          Timestamp = val.Timestamp,
                                          Value = val.Value
                                      }).ToList()

                                  }).ToList<MeterData>();

                    if (result != null && result.Count > 0)
                    {
                        int startRegionRow = 6;
                        int startRegionColumn = 11;

                        MeterData data = result[0];

                        if (data.Source != null && data.Values != null && data.Values.Count > 0)
                        {
                            List<double> values = GetConsuptionValues(data.Values);
                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.FirstOrDefault().Value;

                            }

                            using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.Last().Value;

                            }

                            startRegionRow = 15;
                            startRegionColumn = 7;

                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.LoadFromCollection(values);

                            }
                        }
                        else
                        {
                            logger.Warn(String.Concat(branch.Address, ":",
                                branch.EnergyMeters[meter_index].MarkingPosition, " - ",
                                data.Source, " - Parameter values are missing in the database"));
                        }
                            


                    }
                    #endregion

                    #region Fill Import reactive power
                    result = (from apower in branch.EnergyMeters[meter_index].Parametr
                              where (apower.Source.Contains("Загальний імпорт реактивної енергії"))
                              select new MeterData
                              {
                                  Source = apower.Source,
                                  Values = (
                                      from val in apower.Values
                                      select new TrendValue
                                      {
                                          Timestamp = val.Timestamp,
                                          Value = val.Value
                                      }).ToList()

                              }).ToList<MeterData>();

                    if (result != null && result.Count > 0)
                    {
                        int startRegionRow = 6;
                        int startRegionColumn = 12;

                        MeterData data = result[0];

                        if (data.Source != null && data.Values != null && data.Values.Count > 0)
                        {
                            List<double> values = GetConsuptionValues(data.Values);
                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.FirstOrDefault().Value;

                            }
                            using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.Last().Value;

                            }

                            startRegionRow = 15;
                            startRegionColumn = 8;
                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.LoadFromCollection(values);

                            }
                        }
                        else {
                            logger.Warn(String.Concat(branch.Address, ":",
                            branch.EnergyMeters[meter_index].MarkingPosition, " - ",
                            data.Source, " - Parameter values are missing in the database"));
                        }
                    }
                    
                    #endregion

                    #region Fill Export reactive power
                    result = (from apower in branch.EnergyMeters[meter_index].Parametr
                              where (apower.Source.Contains("Загальний експорт реактивної енергії"))
                              select new MeterData
                              {
                                  Source = apower.Source,
                                  Values = (
                                      from val in apower.Values
                                      select new TrendValue
                                      {
                                          Timestamp = val.Timestamp,
                                          Value = val.Value
                                      }).ToList()

                              }).ToList<MeterData>();

                    if (result != null && result.Count > 0)
                    {
                        int startRegionRow = 6;
                        int startRegionColumn = 13;

                        MeterData data = result[0];
                        if (data.Source != null && data.Values != null && data.Values.Count > 0)
                        {
                            List<double> values = GetConsuptionValues(data.Values);
                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.FirstOrDefault().Value;

                            }
                            using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.Last().Value;

                            }

                            startRegionRow = 15;
                            startRegionColumn = 9;
                            using (ExcelRange range = worksheet.Cells[startRegionRow, (4 * meter_index) + startRegionColumn])
                            {
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.LoadFromCollection(values);

                            }
                        }
                    }
                    #endregion
                }


                bResult = true;
            }
            catch { }

            return bResult;
        }
        private bool FillReportDataWater(ref ExcelWorksheet worksheet, BranchInformation branch, ReportType reportType)
        {
            bool bResult = false;

            try
            {
                for (int meter_index = 0; meter_index < branch.WaterMeters.Count; meter_index++)
                {
                    if (branch.WaterMeters[meter_index].Parametr.Count <= 0)
                    {
                        logger.Info("No parameters saved for metering unit Name : " + branch.EnergyMeters[meter_index].Legend);
                        continue;
                    };

                    #region Fill water consumption
                    var result = (from water in branch.WaterMeters[meter_index].Parametr
                                  where (water.Source.Contains("Показники лічильника"))
                                  select new MeterData
                                  {
                                      Source = water.Source,
                                      Values = (
                                      from val in water.Values
                                      select new TrendValue
                                      {
                                          Timestamp = val.Timestamp,
                                          Value = val.Value
                                      }).ToList()

                                  }).ToList<MeterData>();

                    if (result != null && result.Count > 0)
                    {
                        int startRegionRow = 6;
                        int startRegionColumn = 10;

                        MeterData data = result[0];

                        if (data.Source != null && data.Values != null && data.Values.Count > 0)
                        {
                            List<double> values = GetConsuptionValues(data.Values);
                            List<string> timestamps = GetDateTimeValues(data.Values, reportType);

                            using (ExcelRange range = worksheet.Cells[startRegionRow, (2 * meter_index) + startRegionColumn, startRegionRow, (2 * meter_index) + startRegionColumn + 1])
                            {
                                range.Merge = true;
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.FirstOrDefault().Value;

                            }

                            using (ExcelRange range = worksheet.Cells[startRegionRow + 1, (2 * meter_index) + startRegionColumn, startRegionRow + 1, (2 * meter_index) + startRegionColumn + 1])
                            {
                                range.Merge = true;
                                range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Values.Last().Value;

                            }

                            startRegionRow = 15;
                            startRegionColumn = 3;

                            //using (ExcelRange range = worksheet.Cells[startRegionRow, (2 * meter_index) + startRegionColumn, startRegionRow, (2 * meter_index) + startRegionColumn])
                            //{
                            //    range.Merge = true;
                            //    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                            //    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            //    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            //    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            //    range.Style.Numberformat.Format = "0.00";
                            //    range.LoadFromCollection(values);

                            //}

                            for (int row_index = 0; row_index < timestamps.Count; row_index++)
                            {
                                using (ExcelRange range = worksheet.Cells[startRegionRow + row_index, (2 * meter_index) + startRegionColumn, startRegionRow + row_index, (2 * meter_index) + startRegionColumn + 1])
                                {
                                    range.Merge = true;
                                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    range.Style.Numberformat.Format = "0.00";
                                    range.SetCellValue(0, 0, values[row_index]);
                                }

                                using (ExcelRange range = worksheet.Cells[startRegionRow + row_index, 1])
                                {
                                    range.Style.Font.SetFromFont("Arial", 10, false, false, false, false);
                                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    range.Style.Numberformat.Format = "0.00";
                                    range.SetCellValue(0, 0, timestamps[row_index]);
                                }
                            }

                        }

                    }
                    #endregion

                }

                bResult = true;
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.Source); 
                logger.Error(ex.StackTrace);
            }

            return bResult;
        }



    }

}