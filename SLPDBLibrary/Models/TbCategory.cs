namespace SLPDBLibrary.Models;

public partial class TbCategory
{
    public int Id { get; set; }

    public int? ClimateId { get; set; }

    public int? TypeOfHeatingId { get; set; }

    public int? TreadingSquareId { get; set; }

    public int? BranchSquareId { get; set; }
}
