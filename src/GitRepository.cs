using HansPeterGit.Parser;
using Microsoft.Extensions.Logging;

namespace HansPeterGit;

public class GitRepository
{
    private readonly GitHelper _helper;

    public string WorkingDirectory => _helper.WorkingDirectory;

    public GitRepository(string workingDir, ILogger? logger = null)
    {
        _helper = new GitHelper(workingDir, logger);
    }

    /// <summary>
    /// Clones a repository.
    /// </summary>
    /// <param name="sourceUrl">URI for the remote repository.</param>
    /// <param name="workingDir">Local path to clone into.</param>
    /// <returns></returns>
    public static GitRepository Clone(string sourceUrl, string workingDir, ILogger? logger = null)
    {
        var helperWithoutWorkdir = new GitHelper(string.Empty, logger);

        helperWithoutWorkdir.Command("clone", sourceUrl, workingDir);

        return new GitRepository(workingDir, logger);
    }

    /// <summary>
    /// Initializes a new repository
    /// </summary>
    public void Init()
    {
        Directory.CreateDirectory(WorkingDirectory);

        _helper.Command("init");
    }

    /// <summary>
    /// Initializes a new repository
    /// </summary>
    /// <param name="initialBranchName">Name for the initial branch in the newly created repository</param>
    public void Init(string initialBranchName)
    {
        Directory.CreateDirectory(WorkingDirectory);

        _helper.Command("init", "-b", initialBranchName);
    }

    public void RemoteRemove(string name)
    {
        _helper.Command("remote", "remove", name);
    }

    public void RemoteAdd(string name, string url)
    {
        _helper.Command("remote", "add", name, url);
    }

    public void Fetch(string name)
    {
        _helper.Command("fetch", name);
    }

    public void Reset(string name, bool hard = false)
    {
        var commands = new List<string>();
        commands.Add("reset");

        if (hard)
            commands.Add("--hard");

        commands.Add(name);

        _helper.Command(commands.ToArray()); // HEAD is now at <hash> <message>
    }

    /// <summary>
    /// Executes a "git add . -A"
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void StageAll()
    {
        Add(".", "-A");
    }

    public void Add(string path, string option)
    {
        _helper.Command("add", path, option);
    }

    public RepositoryStatus GetStatus()
    {
        return GetStatus(new StatusOptions());
    }

    public RepositoryStatus GetStatus(StatusOptions options)
    {
        var commands = new List<string>();
        commands.Add("status");
        commands.Add("--porcelain=2");
        commands.Add("--branch");
        if (options.IncludeUntracked)
            commands.Add("--untracked-files=all");
        else
            commands.Add("--untracked-files=no");

        commands.Add("--show-stash");

        if (options.IncludeIgnored)
            commands.Add("--ignored");

        var output = _helper.Command(commands.ToArray());

        if (output == null)
            throw new InvalidOperationException("Git status command failed");

        return StatusParser.ParseOutput(output);
    }

    /// <summary>
    /// Creates a commit with the given message
    /// </summary>
    /// <param name="message">The commit message</param>
    /// <returns>The new commit id</returns>
    public Commit Commit(string message)
    {
        return CommitParser.Parse(_helper.Command("commit", "-m", message));
    }

    /// <summary>
    /// Creates a commit with the given message and author
    /// </summary>
    /// <param name="message">The commit message</param>
    /// <param name="author">The author of this commit</param>
    /// <returns>The new commit id</returns>
    public Commit Commit(string message, Author author)
    {
        return CommitParser.Parse(_helper.Command("commit", "-m", message, $"--author=\"{author}\""));
    }

    /// <summary>
    /// Pushes all local changes.
    /// </summary>
    public void Push()
    {
        _helper.Command("push");
    }


    /// <summary>
    /// Pushes all local changes to the given remote and branch.
    /// </summary>
    /// <param name="remoteName"></param>
    /// <param name="branch"></param>
    public void Push(string remoteName, string branch)
    {
        _helper.Command("push", remoteName, branch);
    }

    /// <summary>
    /// Pushes all local changes and adds an upstream reference.
    /// </summary>
    /// <param name="remoteName"></param>
    /// <param name="branch"></param>
    public void PushWithUpstream(string remoteName, string branch)
    {
        _helper.Command("push", "--set-upstream", remoteName, branch);
    }
}
