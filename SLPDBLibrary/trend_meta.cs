using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPDBLibrary
{
    public class trend_meta
    {
        public int externallogid { get; set; }
        public Guid originatedguid { get; set; }
        public string source { get; set; }
        public Guid serverguid { get; set; }
        public DateTime timeadded { get; set; }
        public bool disabled { get; set; }
        public bool cleared { get; set; }
        public bool pendingexternaldisable { get; set; }
        public bool pendingexternalclear { get; set; }
        public bool pendingexternaldelete { get; set; } 
        public bool pendingexternalenable { get; set;}
        public long retentionperiod { get; set; }
        public long unit { get; set; }

    }
}
