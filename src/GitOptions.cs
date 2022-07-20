using System.Collections.Specialized;
using Microsoft.Extensions.Logging;

namespace HansPeterGit;
public class GitOptions
{
    /// <summary>
    /// Gets or sets the logger instance
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    /// Gets the working directory
    /// </summary>
    public string WorkingDirectory { get; }

    /// <summary>
    /// Gets or sets the path to git executable
    /// </summary>
    public string PathToGit { get; set; } = "git";

    /// <summary>
    /// Action to define environment variables for the git process.
    /// </summary>
    public Action<StringDictionary>? SetEnvironmentVariables { get; set; }

    /// <summary>
    /// Creates new instance of the options
    /// </summary>
    /// <param name="workingDirectory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GitOptions(string workingDirectory)
    {
        WorkingDirectory = workingDirectory ?? throw new ArgumentNullException(nameof(workingDirectory));
    }

    /// <summary>
    /// Creates new instance of the options
    /// </summary>
    /// <param name="workingDirectory"></param>
    /// <param name="logger">The logger instance</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GitOptions(string workingDirectory, ILogger? logger)
        : this(workingDirectory)
    {
        Logger = logger;
    }
}
