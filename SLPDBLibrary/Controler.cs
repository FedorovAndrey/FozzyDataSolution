using MimeKit;
using Org.BouncyCastle.Tls.Crypto;
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
            catch(Exception ex)
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
                var queryResult = (from branche in db.tbBranch
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
        public static List<MailingAddress> GetListMailing(int regionId)
        {
            List<MailingAddress> lMailingList = new List<MailingAddress>();

            try {
                using(DatabaseContext db = new DatabaseContext()) {
                    
                    var query = (from employee in db.tbEmployees 
                                where employee.Mailing == regionId ||
                                employee.Mailing == 0 
                                 select employee).ToList();

                    foreach(var employee in query) 
                    {

                        MailingAddress mailAddress = new() { Name = String.Concat(employee.FirstName, " ", employee.LastName), Mail = employee.Email};
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
        public static List<object> GetMeterReport(DateTime timestamp_begin, DateTime timestamp_end)
        {
            List<object> list = new List<object>();

            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    var query = (from item in db.trend_datas
                                 where item.timestamp > timestamp_begin.AddHours (-1) &&
                                 item.timestamp < timestamp_begin.AddHours(1)
                                 select item).ToList();
                }
            }
            catch (Exception ex)
            { 

            }
            return list;
        }

    }
}
