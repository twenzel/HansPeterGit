namespace HansPeterGit;

/// <summary>
/// Contains git commit information
/// </summary>
/// <param name="Id">The commit id (sha)</param>
/// <param name="Branch">The branch name</param>
/// <param name="Message">The commit message</param>
public record Commit(string Id, string Branch, string Message);
