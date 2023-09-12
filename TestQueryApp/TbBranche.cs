using System;
using System.Collections.Generic;

namespace TestQueryApp;

public partial class TbBranche
{
    public int Id { get; set; }

    public int? Category { get; set; }

    public int? City { get; set; }

    public string? Address { get; set; }

    public string? ServerName { get; set; }

    public int? Mvz { get; set; }

    public float? BranchSquare { get; set; }

    public float? TradingSquare { get; set; }

    public int? Region { get; set; }
}
