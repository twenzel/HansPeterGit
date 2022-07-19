using System.Diagnostics;

namespace HansPeterGit;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class StatusEntry
{
    public FileStatus State { get; set; }
    public string FilePath { get; init; } = string.Empty;

    private string DebuggerDisplay => $"{State}: {FilePath}";
}
