using System;
using System.Collections.Generic;

namespace TestQueryApp;

public partial class TrendMetum
{
    public int Externallogid { get; set; }

    public Guid? Originatedguid { get; set; }

    public string? Source { get; set; }

    public Guid? Serverguid { get; set; }

    public DateTime? Timeadded { get; set; }

    public bool? Disabled { get; set; }

    public bool? Cleared { get; set; }

    public bool? Pendingexternaldisable { get; set; }

    public bool? Pendingexternalclear { get; set; }

    public bool? Pendingexternaldelete { get; set; }

    public bool? Pendingexternalenable { get; set; }

    public long? Retentionperiod { get; set; }

    public long? Unit { get; set; }

    public virtual ICollection<Hyper22Chunk> Hyper22Chunks { get; set; } = new List<Hyper22Chunk>();

    public virtual ICollection<TrendDatum> TrendData { get; set; } = new List<TrendDatum>();
}
