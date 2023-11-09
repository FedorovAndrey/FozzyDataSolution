namespace SLPDBLibrary.Models;

public partial class TbMeter
{
    public int Id { get; set; }

    public int BranchId { get; set; }

    public string? Vendor { get; set; }

    public string? Model { get; set; }

    public string? SerialNumber { get; set; }

    public string? MarkingPosition { get; set; }

    public int? RoleId { get; set; }

    public string? Legend { get; set; }

    public int? TypeId { get; set; }
}
