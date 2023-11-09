using NLog;
using SLPDBLibrary.Models;

namespace SLPDBLibrary
{
    public static class Controler
    {
        private static Logger logger = LogManager.GetLogger("logger");
        public static List<TbRegion> GetRegion()
        {
            List<TbRegion> listRegion = new List<TbRegion>();
            try
            {
                using (EboDbContext db = new EboDbContext())
                {
                    var queryRegion = from regions in db.TbRegions select regions;
                    listRegion = queryRegion.ToList<TbRegion>();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return listRegion;
        }
        public static List<BranchInformation> GetBranchesInformation(int regionId)
        {
            List<BranchInformation> branches = new List<BranchInformation>();

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
                                       Meters = (from meters in db.TbMeters where meters.BranchId == branche.Id select meters).ToList<TbMeter>()

                                   });

                foreach (var item in queryResult)
                {
                    BranchInformation branchInformation = new BranchInformation();

                    branchInformation.id = item.id;
                    branchInformation.Region = item.Region;
                    branchInformation.City = item.City;
                    branchInformation.Address = item.Address;
                    branchInformation.ServerName = item.ServerName;
                    branchInformation.meterCount = item.Meters.Count();

                    foreach (TbMeter meter in item.Meters)
                    {
                        branchInformation.Meters.Add(
                            new Meter
                            {
                                Legend = meter.Legend,
                                MarkingPosition = meter.MarkingPosition,
                                Model = meter.Model,
                                SerialNumber = meter.SerialNumber,
                                Vendor = meter.Vendor
                            });
                    }

                    branches.Add(branchInformation);
                }

            }

            return branches;
        }
        public static List<TbMeter> GetMeterByBranchId(int branchID)
        {
            List<TbMeter> result = new List<TbMeter>();

            using (EboDbContext db = new EboDbContext())
            {
                var queryResult = (from meters in db.TbMeters
                                   where meters.BranchId == branchID
                                   select meters);
                result = queryResult.ToList();
            }
            return result;
        }
        public static List<MailingAddress> GetListMailing(int regionId)
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
            catch (Exception e)
            {
                return null;
            }
            return lMailingList;
        }
        public static bool GetMeterReport(ref BranchInformation branch, DateTime timestamp_begin, DateTime timestamp_end)
        {
            /*
             * /emon001-ES/BranchServer/o-cr-gvard1-em1/General/TrendLog/Експорт активної потужності - Загальна
             */
            bool bResult = false;

            try
            {
                if (branch.Meters.Count <= 0)
                {
                    logger.Warn(String.Concat("Branch ", branch.Address, " does not contain a list of metering units."));
                    return true;
                }
                foreach (var meter in branch.Meters)
                {
                    string s_server = branch.ServerName.Replace("{", "").Replace("}", "");
                    string s_meter = meter.MarkingPosition.Replace("-", "");

                    using (EboDbContext db = new EboDbContext())
                    {
                        var query = (from trend in db.TrendMeta
                                     where trend.Source.Contains(s_server) &&
                                     trend.Source.Contains(s_meter) &&
                                     trend.Source.Contains("Загальний") &&
                                     (trend.Source.Contains("експорт") ||
                                      trend.Source.Contains("імпорт"))
                                      orderby trend.Source 
                                     select new
                                     {
                                         source = trend.Source,
                                         trend_id = trend.Externallogid,
                                         data = (from data in db.TrendData where (
                                                 data.Externallogid == trend.Externallogid && 
                                                 data.Timestamp >= timestamp_begin && 
                                                 data.Timestamp <= timestamp_end &&
                                                 data.Timestamp.Minute == 0 ) 
                                                 select data).ToList()
                                     }).ToList();

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

    }
}
