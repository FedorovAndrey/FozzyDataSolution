using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPDBLibrary
{
    public class tbBranch
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int RegionID { get; set; }
        public int CityID { get; set; }
        public string? Address { get; set; }
        public string? ServerName { get; set; }
        public int MVZ { get; set; }
        public float BranchSquare { get; set; }
        public float TradingSquare { get; set; }

    }
}
