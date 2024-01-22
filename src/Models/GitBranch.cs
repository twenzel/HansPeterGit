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

    /// <summary>
    /// Gets the current Commit hash
    /// </summary>
    public string CommitHash { get; init; } = string.Empty;

    /// <summary>
    /// Gets the current commit message
    /// </summary>
    public string CommitMessage { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the remote branch
    /// </summary>
    public string RemoteBranch { get; init; } = string.Empty;
}
