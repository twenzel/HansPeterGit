namespace HansPeterGit;

/// <summary>
/// Helper class for global Git operations
/// </summary>
public static class GitGlobal
{
    /// <summary>
    /// Changes the global git configuration
    /// </summary>
    /// <param name="configName">Name of the configuration entry</param>
    /// <param name="value">The new value</param>
    /// <param name="options">The git options.</param>
    public static void Config(string configName, string value, GitOptions options)
    {
        var helperWithoutWorkdir = new GitHelper(options with { WorkingDirectory = string.Empty });

        helperWithoutWorkdir.Command("config", "--global", configName, value);
    }
}
