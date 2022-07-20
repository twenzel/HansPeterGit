namespace HansPeterGit;

/// <summary>
/// Options to control the behavior of the status command
/// </summary>
public class StatusOptions
{
    /// <summary>
    /// Include unaltered files when scanning for status
    /// </summary>
    /// <remarks>Unaltered meaning the file is identical in the working directory, the index and HEAD.</remarks>
    public bool IncludeUnaltered { get; set; } = false;

    /// <summary>
    /// Include ignored files when scanning for status
    /// </summary>
    /// <remarks>ignored meaning present in .gitignore. </remarks>
    public bool IncludeIgnored { get; set; } = false;

    /// <summary>
    /// Include untracked files when scanning for status.
    /// </summary>
    public bool IncludeUntracked { get; set; } = true;
}
