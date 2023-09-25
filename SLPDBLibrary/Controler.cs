using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPDBLibrary
{
    public static class Controler
    {
        public static List<tbRegions> GetRegion()
        {
            List<tbRegions> listRegion = new List<tbRegions>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    var queryRegion = from regions in db.tbRegions select regions;
                    listRegion = queryRegion.ToList<tbRegions>();
                }
            }
            catch
            {
                return null;
            }
            return listRegion;
        }
        public static List<BranchInformation> GetBranchesInformation(int regionId)
        {
            List<BranchInformation> branches = new List<BranchInformation>();

            using (DatabaseContext db = new DatabaseContext())
            {
                var queryResult = (from branche in db.tbBranche
                                   join city in db.tbCities on branche.CityID equals city.ID
                                   join region in db.tbRegions on branche.RegionID equals region.ID
                                   where branche.RegionID == regionId
                                   orderby branche.ID
                                   select new
                                   {
                                       id = branche.ID,
                                       Region = region.Name,
                                       City = city.Name,
                                       Address = branche.Address,
                                       MeterCount = (from meter in db.tbMeters where meter.BranchId == branche.ID select meter).Count()

                                   });;

                foreach (var item in queryResult)
                {
                    branches.Add(new BranchInformation { id = item.id, Region = item.Region, City = item.City, Address = item.Address, meterCount = item.MeterCount });
                }
                

            }

            return branches;
        }
        public static List<tbMeters> GetMeterByBranchId(int branchID)
        {
            List<tbMeters> result = new List<tbMeters>();

            using (DatabaseContext db = new DatabaseContext())
            {
                var queryResult = (from meters in db.tbMeters
                                   where meters.BranchId == branchID
                                   select meters);
                result = queryResult.ToList();
            }
            return result;
        }
    }
}
