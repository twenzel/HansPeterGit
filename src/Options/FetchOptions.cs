namespace HansPeterGit.Options;

/// <summary>
/// Options for the fetch command
/// </summary>
public class FetchOptions : GitCommandOptions
{
    /// <summary>
    /// Gets or sets the name of the remote to fetch from.
    /// Default is <c>null</c>
    /// </summary>
    [GitOption]
    public string? Remote { get; set; }

    /// <summary>
    /// Gets or sets whether to prune during fetch.
    /// Default is <c>null</c>
    /// </summary>
    [GitOption("--prune")]
    public bool? Prune { get; set; }

    /// <summary>
    /// Fetch all remotes.
    /// Default is <c>null</c>
    /// </summary>
    [GitOption("--all")]
    public bool? All { get; set; }

    /// <summary>
    /// Fetch all tags from the remote.
    /// Default is <c>null</c>
    /// </summary>
    [GitOption("--tags")]
    public bool? Tags { get; set; }

    /// <summary>
    /// Before fetching, remove any local tags that no longer exist on the remote if --prune is enabled.
    /// Default is <c>null</c>
    /// </summary>
    [GitOption("--prune-tags")]
    public bool? PruneTags { get; set; }
}
