namespace HansPeterGit;

/// <summary>
/// Defines a git author
/// </summary>
/// <param name="Name">User name of the author</param>
/// <param name="Email">Email address of the author</param>
public record Author(string Name, string Email)
{
    /// <summary>
    /// Returns the git author expression
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Name} <{Email}>";
}
