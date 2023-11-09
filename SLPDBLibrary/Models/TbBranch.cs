namespace SLPDBLibrary.Models;

public partial class TbBranch
{
    public int Id { get; set; }

    public int? CityId { get; set; }

    public int? CategoryId { get; set; }

    public int? RegionId { get; set; }

    public string? Address { get; set; }

    public string? ServerName { get; set; }

    public string? Mvz { get; set; }

    public float? BranchSquare { get; set; }

    public float? TradingSquare { get; set; }
}
