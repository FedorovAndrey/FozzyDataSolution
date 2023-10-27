using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPDBLibrary
{
    public class trend_data
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long externalseqno { get; set; }
        public string comment { get; set; }
        public string description { get; set; }
        public DateTime edittime { get; set; }
        public short edittime_tzinfo { get;set;}
        public DateTime endtime { get; set; }   
        public short endtime_tzinfo { get; set; }
        public double endvalue { get; set; }
        //public short event{ get;set; }
        public int externallogid { get; set; }  
        public string logtype { get; set; } 
        public double maxvalue { get; set; }
        public double meterconstant { get; set; }
        public double minvalue { get; set;}
        public long originalseqno { get; set; }
        public long refseqno { get; set; }

        public long seqno { get;set; }
        public Guid serverguid { get;set; }
        public DateTime starttime { get; set; }
        public short starttime_tzinfo { get; set; }
        public long startvalue { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public DateTime timestamp { get; set; }
        public short timestamp_tzinfo { get; set; }
        public string username { get; set; }
        public double value { get;protected set; }  




    }
}
