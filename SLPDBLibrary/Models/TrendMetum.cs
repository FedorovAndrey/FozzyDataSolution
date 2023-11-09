namespace SLPDBLibrary.Models;

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

    public virtual ICollection<Hyper210Chunk> Hyper210Chunks { get; set; } = new List<Hyper210Chunk>();

    public virtual ICollection<Hyper21Chunk> Hyper21Chunks { get; set; } = new List<Hyper21Chunk>();

    public virtual ICollection<Hyper22Chunk> Hyper22Chunks { get; set; } = new List<Hyper22Chunk>();

    public virtual ICollection<Hyper23Chunk> Hyper23Chunks { get; set; } = new List<Hyper23Chunk>();

    public virtual ICollection<Hyper24Chunk> Hyper24Chunks { get; set; } = new List<Hyper24Chunk>();

    public virtual ICollection<Hyper25Chunk> Hyper25Chunks { get; set; } = new List<Hyper25Chunk>();

    public virtual ICollection<Hyper26Chunk> Hyper26Chunks { get; set; } = new List<Hyper26Chunk>();

    public virtual ICollection<Hyper27Chunk> Hyper27Chunks { get; set; } = new List<Hyper27Chunk>();

    public virtual ICollection<Hyper28Chunk> Hyper28Chunks { get; set; } = new List<Hyper28Chunk>();

    public virtual ICollection<Hyper29Chunk> Hyper29Chunks { get; set; } = new List<Hyper29Chunk>();

    public virtual ICollection<TrendDatum> TrendData { get; set; } = new List<TrendDatum>();
}
