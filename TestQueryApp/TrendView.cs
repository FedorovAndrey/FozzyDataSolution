using System;
using System.Collections.Generic;

namespace TestQueryApp;

public partial class TrendView
{
    public long? Externalseqno { get; set; }

    public string? Comment { get; set; }

    public string? Description { get; set; }

    public DateTime? Edittime { get; set; }

    public short? EdittimeTzinfo { get; set; }

    public DateTime? Endtime { get; set; }

    public short? EndtimeTzinfo { get; set; }

    public double? Endvalue { get; set; }

    public short? Event { get; set; }

    public int? Externallogid { get; set; }

    public string? Logtype { get; set; }

    public double? Maxvalue { get; set; }

    public double? Meterconstant { get; set; }

    public double? Minvalue { get; set; }

    public long? Originalseqno { get; set; }

    public long? Refseqno { get; set; }

    public long? Seqno { get; set; }

    public Guid? Serverguid { get; set; }

    public DateTime? Starttime { get; set; }

    public short? StarttimeTzinfo { get; set; }

    public double? Startvalue { get; set; }

    public int? Status { get; set; }

    public int? Type { get; set; }

    public DateTime? Timestamp { get; set; }

    public short? TimestampTzinfo { get; set; }

    public string? Username { get; set; }

    public double? Value { get; set; }
}
