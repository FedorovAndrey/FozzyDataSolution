using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLPDBLibrary.Models;

namespace SLPDBLibrary
{
    public class MeterData
    {
        public string? Source { get; set; }
        public int SourceId { get; set; }
        public TrendDatum[]? values {get;set; }
    }
}
