using System;
using System.Collections.Generic;

namespace TestQueryApp;

public partial class Unit
{
    public string? Unitdisplayname { get; set; }

    public string? Unitdescription { get; set; }

    public int Unitid { get; set; }

    public double? Conversionfactor { get; set; }

    public string? Typesystem { get; set; }

    public int? Prefixfactor { get; set; }

    public string? Categorydisplayname { get; set; }

    public int Categoryid { get; set; }

    public string Languageid { get; set; } = null!;
}
