using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPDBLibrary
{
    public class tbBranche
    {
        public int ID { get; set; }
        public int Category { get; set; }
        public int City { get; set; }
        public string? Address { get; set; }
        public string? ServerName { get; set; }
        public int MVZ { get; set; }
        public float BranchSquare { get; set; }
        public float TradingSquare { get; set; }
        public int Region { get; set; }
    }
}
