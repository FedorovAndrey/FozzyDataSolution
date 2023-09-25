using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace SLPDBLibrary
{
    public class tbMeters
    {
        public int ID { get; set; }
        public int BranchId { get; set; }
        public int Role { get; set; }
        public string? Vendor { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? MarkingPosition { get; set; }
        public string? Legend { get; set; }
    }
}
