using System.Data.Entity;
using NLog;
using SLPDBLibrary.Models;
using SLPHelper;


namespace SLPDBLibrary
{
    public static class Controler
    {
        private static Logger logger = LogManager.GetLogger("logger");
        public static List<Region>? GetRegion()
        {
            List<Region> listRegion = new List<Region>();
            try
            {
                using (EboDbContext db = new EboDbContext())
                {
                    var queryRegion = (from regions in db.TbRegions
                                       select new Region
                                       {
                                           ID = regions.Id,
                                           Name = regions.Name
                                       });
                    listRegion.AddRange(queryRegion);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.Source);
                return null;
            }
            return listRegion;
        }
        public static bool GetBranchesInformation( ref List<BranchInformation> branches,  int regionId)
        {
            bool bResult = false;

            try
            {
                using (EboDbContext db = new EboDbContext())
                {
                    var queryResult = (from branche in db.TbBranches
                                       join city in db.TbCities on branche.CityId equals city.Id
                                       join region in db.TbRegions on branche.RegionId equals region.Id
                                       where branche.RegionId == regionId
                                       orderby branche.Id
                                       select new
                                       {
                                           id = branche.Id,
                                           Region = region.Name,
                                           City = city.Name,
                                           Address = branche.Address,
                                           ServerName = branche.ServerName,
                                           EnergyMeters = db.TbMeters
                                                        .Where(meter => meter.BranchId == branche.Id && meter.TypeId == 2)
                                                        .OrderBy(meter => meter.MarkingPosition)
                                                        .ToList(),
                                           WaterMeters = db.TbMeters
                                                        .Where(meter => meter.BranchId == branche.Id && meter.TypeId == 3)
                                                        .OrderBy(meter => meter.MarkingPosition)
                                                        .ToList()
                                       });

                    foreach (var item in queryResult)
                    {
 
                        BranchInformation branchInformation = new BranchInformation
                        {
                            id = item.id,
                            Region = item.Region,
                            City = item.City,
                            Address = item.Address,
                            ServerName = item.ServerName
                        };
                        branchInformation.EnergyMeters.AddRange(item.EnergyMeters.Select(meter => new Meter
                        {
                            Legend = meter.Legend,
                            MarkingPosition = meter.MarkingPosition,
                            Model = meter.Model,
                            SerialNumber = meter.SerialNumber,
                            Vendor = meter.Vendor
                        }));
                        branchInformation.WaterMeters.AddRange(item.WaterMeters.Select(meter => new Meter
                        {
                            Legend = meter.Legend,
                            MarkingPosition = meter.MarkingPosition,
                            Model = meter.Model,
                            SerialNumber = meter.SerialNumber,
                            Vendor = meter.Vendor
                        }));

                        branches.Add(branchInformation);
                    }
                }

                    bResult = true;
            }
            catch (Exception ex)
            { 
                logger.Error(ex.Message);
                logger.Error(ex.Source);
                logger.Error(ex.StackTrace);    
            }

            return bResult;

        }
        public static List<TbMeter> GetMeterByBranchId(int branchID)
        {
            List<TbMeter> result = new List<TbMeter>();

            using (EboDbContext db = new EboDbContext())
            {
                var queryResult = (from meters in db.TbMeters
                                   where meters.BranchId == branchID
                                   orderby meters.MarkingPosition
                                   select meters);
                result = queryResult.ToList();
                if (result.Count > 0 && result[0].MarkingPosition != "WH-01")
                {

                }
            }
            return result;
        }
        public static List<MailingAddress>? GetListMailing(int regionId)
        {
            List<MailingAddress> lMailingList = new List<MailingAddress>();

            try
            {
                using (EboDbContext db = new EboDbContext())
                {

                    var query = (from employee in db.TbEmployees
                                 where employee.Mailing == regionId ||
                                 employee.Mailing == 0
                                 select employee).ToList();

                    foreach (var employee in query)
                    {


                        MailingAddress mailAddress = new() { Name = String.Concat(employee.FirstName, " ", employee.LastName), Mail = employee.Email };
                        lMailingList.Add(mailAddress);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.Source);
                return null;
            }
            return lMailingList;
        }
        public static bool GetMeterData(ref List<Meter> meters, string server, ReportType reportType, EnergyResource resource, DateTime timestamp_begin, DateTime timestamp_end)
        {
            //*
            //* /emon001-ES/BranchServer/o-cr-gvard1-em1/General/TrendLog/Експорт активної потужності - Загальна
            //* /emon001-ES/BranchServer/o-cr-gvard1-em1/QS01/TrendLog/QS01-Показники лічильника
            //* /emon001-ES/BranchServer/o-zh-zhitn1-em1/WH01/TrendLog/Повна потужність - Фаза С
            //
            bool bResult = false;

            try
            {
                
                foreach (var meter in meters)
                {
                    string s_server = server.Replace("{", "").Replace("}", "");
                    string s_meter = meter.MarkingPosition.Replace("-", "");
                    /*
                    if (s_server == "o-zh-zhitn1-em1")
                    {
                        using (EboDbContext db = new EboDbContext())
                        {
                            var testQuery = (from trend in db.TrendMeta
                                             where trend.Source.Contains(s_server) &&
                                             trend.Source.Contains(s_meter) &&
                                             trend.Source.Contains("Загальний") &&
                                             (trend.Source.Contains("експорт") || trend.Source.Contains("імпорт"))
                                             orderby trend.Source
                                             select new MeterData
                                             {
                                                 Source = trend.Source,
                                                 SourceId = trend.Externallogid,
                                                 Values = (from data in db.TrendData
                                                            where data.Externallogid == trend.Externallogid &&
                                                            data.Timestamp.Minute == 0
                                                           select new TrendValue
                                                           {
                                                               Timestamp = data.Timestamp,
                                                               Value = data.Value
                                                           }).ToList()

                                             }); ; 
                        }

                    }
                    */
                    
                    if (resource == EnergyResource.Energy)
                    {
                        using (EboDbContext db = new EboDbContext())
                        {
                            var query =(
                                from trend in db.TrendMeta
                                where trend.Source.Contains(s_server) &&
                                      trend.Source.Contains(s_meter) &&
                                      trend.Source.Contains("Загальний") &&
                                      (trend.Source.Contains("експорт") || trend.Source.Contains("імпорт"))
                                orderby trend.Source
                                select new MeterData
                                {
                                    Source = trend.Source,
                                    SourceId = trend.Externallogid,

                                    Values = (
                                        from data in db.TrendData
                                        where data.Externallogid == trend.Externallogid &&
                                              data.Timestamp >= timestamp_begin &&
                                              data.Timestamp <= timestamp_end &&
                                              data.Timestamp.Minute == 0
                                        select new TrendValue
                                        {
                                            Timestamp = data.Timestamp,
                                            Value = data.Value
                                        }
                                    ).ToList()
                                }
                                ).ToList();
                            meter.Parametr.AddRange(query);

                        }
                    }

                    if (resource == EnergyResource.Water)
                    {
                        using (EboDbContext dbContext = new EboDbContext())
                        {
                            var query = (
                                from trend in dbContext.TrendMeta
                                where trend.Source.Contains(s_server) &&
                                      trend.Source.Contains(s_meter) &&
                                      trend.Source.Contains("Показники") &&
                                      trend.Source.Contains("лічильника")
                                orderby trend.Source
                                select new MeterData
                                {
                                    Source = trend.Source,
                                    SourceId = trend.Externallogid,

                                    Values = (
                                        from data in dbContext.TrendData
                                        where data.Externallogid == trend.Externallogid &&
                                              data.Timestamp >= timestamp_begin &&
                                              data.Timestamp <= timestamp_end &&
                                              data.Timestamp.Minute == 0
                                        select new TrendValue
                                        {
                                            Timestamp = data.Timestamp,
                                            Value = data.Value
                                        }
                                    ).ToList()
                                }
                                ).ToList();
                            meter.Parametr.AddRange(query);

                        }
                    }
                }
                bResult = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }


            return bResult;
        }
        public static bool GetMeterDataWeekly()
        {
            bool bResult = false;

            try { 
                
                bResult = true;
            }
            catch(Exception ex) {

                logger.Error(ex.Message);
                logger.Error(ex.Source);
            }

            return bResult;
        }
        
    }
}
