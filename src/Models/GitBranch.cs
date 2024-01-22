namespace HansPeterGit.Models;

/// <summary>
/// Contains information about a branch
/// </summary>
public record GitBranch
{
    /// <summary>
    /// Gets the branch name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// True if this is a remote branch
    /// </summary>
    public bool IsRemote { get; init; }

    /// <summary>
    /// Gets whether this is the current local branch
    /// </summary>
    public bool IsCurrentLocalBranch { get; init; }
}
