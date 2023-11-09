namespace SLPDBLibrary.Models;

public partial class TbEmployee
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public string? Position { get; set; }

    public string? Email { get; set; }

    public int? Mailing { get; set; }
}
