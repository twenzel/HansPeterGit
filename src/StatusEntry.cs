using System.Diagnostics;

namespace HansPeterGit;

/// <summary>
/// Defines a single entry in the repository status
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class StatusEntry
{
    /// <summary>
    /// Gets or sets the state of the file
    /// </summary>
    public FileStatus State { get; set; }

    /// <summary>
    /// Gets or sets the path of the file
    /// </summary>
    public string FilePath { get; init; } = string.Empty;

    private string DebuggerDisplay => $"{State}: {FilePath}";
}
