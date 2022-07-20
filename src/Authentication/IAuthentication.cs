using System.Diagnostics;

namespace HansPeterGit.Authentication;

/// <summary>
/// Interface to provide authentication for git operations
/// </summary>
public interface IAuthentication
{
    /// <summary>
    /// Add authentication to the git process
    /// </summary>
    /// <param name="startInfo">The git process start info.</param>
    void AddAuthentication(ProcessStartInfo startInfo);
}
