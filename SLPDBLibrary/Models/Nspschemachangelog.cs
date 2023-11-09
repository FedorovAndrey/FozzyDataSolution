namespace SLPDBLibrary.Models;

/// <summary>
/// Table to keep track of schema changes
/// </summary>
public partial class Nspschemachangelog
{
    /// <summary>
    /// Current internal version of the schema
    /// </summary>
    public int CurrentInternalVersion { get; set; }

    /// <summary>
    /// Min supported internal version
    /// </summary>
    public int? MinSupportedInternalVersion { get; set; }

    /// <summary>
    /// Timestamp when the change was applied
    /// </summary>
    public DateTime? TimeApplied { get; set; }
}
