using HansPeterGit.Models;
using HansPeterGit.Options;
using HansPeterGit.Parser;
using Microsoft.Extensions.Logging;

namespace HansPeterGit;

/// <summary>
/// Abstracts the git repository and provides interaction functionality
/// </summary>
public class GitRepository
{
    private readonly GitHelper _helper;

    /// <summary>
    /// Gets the current working directory of the Git process.
    /// </summary>
    public string WorkingDirectory => _helper.WorkingDirectory;

    /// <summary>
    /// Creates a new instance of the repository.
    /// </summary>
    /// <param name="workingDir">Local path to of the repository.</param>
    /// <param name="logger">The logger instance.</param>
    public GitRepository(string workingDir, ILogger? logger = null)
        : this(new GitOptions(workingDir, logger))
    {

    }

    /// <summary>
    /// Creates a new instance of the repository.
    /// </summary>
    /// <param name="options">The git options.</param>
    public GitRepository(GitOptions options)
    {
        _helper = new GitHelper(options);
    }

    /// <summary>
    /// Clones a repository.
    /// </summary>
    /// <param name="sourceUrl">URI for the remote repository.</param>
    /// <param name="workingDir">Local path to clone into.</param>
    /// <param name="logger">The logger instance</param>
    /// <returns></returns>
    public static GitRepository Clone(string sourceUrl, string workingDir, ILogger? logger = null)
    {
        return Clone(sourceUrl, new GitOptions(workingDir, logger));
    }

    /// <summary>
    /// Clones a repository.
    /// </summary>
    /// <param name="sourceUrl">URI for the remote repository.</param>
    /// <param name="options">The git options.</param>
    /// <returns></returns>
    public static GitRepository Clone(string sourceUrl, GitOptions options)
    {
        var helperWithoutWorkdir = new GitHelper(options with { WorkingDirectory = string.Empty });

        helperWithoutWorkdir.Command("clone", sourceUrl, options.WorkingDirectory);

        return new GitRepository(options);
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

    /// <summary>
    /// Remove the given remote
    /// </summary>
    /// <param name="name">Name of the remote</param>
    public void RemoteRemove(string name)
    {
        _helper.Command("remote", "remove", name);
    }

    /// <summary>
    /// Add a new remote
    /// </summary>
    /// <param name="name">Name of the remote</param>
    /// <param name="url">Url of the remote repository</param>
    public void RemoteAdd(string name, string url)
    {
        _helper.Command("remote", "add", name, url);
    }

    /// <summary>
    /// Fetches a remote
    /// </summary>
    /// <param name="name">Name of the remote</param>
    public void Fetch(string name)
    {
        _helper.Command("fetch", name);
    }

    /// <summary>
    /// Fetches a remote
    /// </summary>
    public void Fetch(FetchOptions? options)
    {
        _helper.Command("fetch", options);
    }

    /// <summary>
    /// Fetch from and integrate with another repository or a local branch
    /// </summary>
    public void Pull()
    {
        _helper.Command("pull");
    }

    /// <summary>
    /// Fetch from and integrate with another repository or a local branch
    /// </summary>
    /// <param name="options">Options for the pull command</param>
    public void Pull(params string[] options)
    {
        var commandOptions = new List<string>();
        commandOptions.Add("pull");

        if (options != null && options.Length > 0)
            commandOptions.AddRange(options);

        _helper.Command(commandOptions.ToArray());
    }

    /// <summary>
    /// Remove untracked files from the working tree
    /// </summary>
    public void Clean()
    {
        _helper.Command("clean");
    }

    /// <summary>
    /// Remove untracked files from the working tree
    /// </summary>
    /// <param name="options">Options for the clean command</param>
    public void Clean(params string[] options)
    {
        var commandOptions = new List<string>();
        commandOptions.Add("clean");

        if (options != null && options.Length > 0)
            commandOptions.AddRange(options);

        _helper.Command(commandOptions.ToArray());
    }

    /// <summary>
    /// Executes a "git clean -fdx"
    /// </summary>
    public void CleanAll()
    {
        Clean("-fdx");
    }

    /// <summary>
    /// Resets the current HEAD to the given commit
    /// </summary>
    /// <param name="name">Name of the commit</param>
    /// <param name="hard">True for hard reset</param>
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
    public void StageAll()
    {
        Add(".", "-A");
    }

    /// <summary>
    /// Stage the given file
    /// </summary>
    /// <param name="path">Path of the file to stage</param>
    public void Add(string path)
    {
        _helper.Command("add", path);
    }

    /// <summary>
    /// Stage the given file
    /// </summary>
    /// <param name="path">Path of the file to stage</param>
    /// <param name="option">Staging options</param>
    public void Add(string path, string option)
    {
        _helper.Command("add", path, option);
    }

    /// <summary>
    /// Restore the given file
    /// </summary>
    /// <param name="path">Path of the file to restore</param>
    public void Restore(string path)
    {
        _helper.Command("restore", path);
    }

    /// <summary>
    /// Unstage the given file
    /// </summary>
    /// <param name="path">Path of the file to unstage</param>
    public void Unstage(string path)
    {
        _helper.Command("restore", "--staged", path);
    }

    /// <summary>
    /// Gets the local branches of the repository.
    /// </summary>
    /// <returns>A list with the branch.</returns>
    public IEnumerable<GitBranch> GetBranches()
    {
        return GetBranches(false);
    }

    /// <summary>
    /// Gets the local or remote branches of the repository.
    /// </summary>
    /// <param name="remotes"><c>true</c> if the remote branches shall be retrieved; <c>false</c> to retrieve the local
    /// branches.</param>
    /// <param name="includeDetails"><c>true</c> to include detail informations of the branches</param>
    /// <returns>A list with the branches.</returns>
    public IEnumerable<GitBranch> GetBranches(bool remotes, bool includeDetails = true)
    {
        var branches = new List<GitBranch>();
        var commands = new List<string> { "branch" };
        if (remotes && includeDetails)
            commands.Add("-vvr");
        else if (remotes)
            commands.Add("-r");
        else if (includeDetails)
            commands.Add("-vv");

        var output = _helper.Command(commands.ToArray());
        if (output == null)
            return branches;

        var matches = output.Split('\n');

        foreach (var match in matches)
        {
            var branch = match.Trim();
            if (!string.IsNullOrWhiteSpace(branch) && !branch.Contains(" -> "))
                branches.Add(CreateBranchInfo(remotes, branch));
        }

        return branches;
    }

    private static GitBranch CreateBranchInfo(bool remotes, string branch)
    {
        var hash = string.Empty;
        var message = string.Empty;
        var remote = string.Empty;
        var isCurrentLocalBranch = false;
        var branchName = branch;

        if (branch.StartsWith("* "))
        {
            isCurrentLocalBranch = true;
            branch = branch.Substring(2);
            branchName = branch;
        }

        if (branch.Contains(' '))
        {
            var index = branch.IndexOf(' ');
            branchName = branch.Substring(0, index);
            branch = branch.Substring(index + 1).Trim();

            index = branch.IndexOf(' ');

            if (index > 0)
            {
                hash = branch.Substring(0, index);
                branch = branch.Substring(index + 1);
            }

            if (branch.StartsWith("["))
            {
                index = branch.IndexOf(']');
                remote = branch.Substring(1, index - 1);
                branch = branch.Substring(index + 2);
            }

            if (branch.Length > 0)
                message = branch;
        }

        return new GitBranch
        {
            Name = branchName,
            IsCurrentLocalBranch = isCurrentLocalBranch,
            IsRemote = remotes,
            CommitHash = hash,
            CommitMessage = message,
            RemoteBranch = remote
        };
    }

    /// <summary>
    /// Gets the local branches of the repository.
    /// </summary>
    /// <returns>A list with the branch names.</returns>
    public IEnumerable<string> GetBranchNames() => GetBranchNames(false);

    /// <summary>
    /// Gets the local or remote branches of the repository.
    /// </summary>
    /// <param name="remotes"><c>true</c> if the remote branches shall be retrieved; <c>false</c> to retrieve the local
    /// branches.</param>
    /// <returns>A list with the branch names.</returns>
    public IEnumerable<string> GetBranchNames(bool remotes) => GetBranches(remotes, false).Select(branch => branch.Name);

    /// <summary>
    /// Gets the current repository status. Executed with default options.
    /// </summary>
    /// <returns></returns>
    public RepositoryStatus GetStatus()
    {
        return GetStatus(new StatusOptions());
    }

    /// <summary>
    /// Gets the current repository status using the given options.
    /// </summary>
    /// <param name="options">Options to retrieve the repository status.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

        return StatusParser.Parse(output);
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

    /// <summary>
    /// Switch the current branch
    /// </summary>
    /// <param name="branch">The branch to switch to.</param>
    public void Switch(string branch)
    {
        _helper.Command("switch", branch);
    }

    /// <summary>
    /// Switch the current branch
    /// </summary>
    /// <param name="branch">The branch to switch to.</param>
    public void Checkout(string branch)
    {
        _helper.Command("checkout", branch);
    }

    /// <summary>
    /// Switch the current branch
    /// </summary>
    /// <param name="branch">The branch to switch to.</param>
    /// <param name="options">Options for the checkout command</param>
    public void Checkout(string branch, params string[] options)
    {
        var commandOptions = new List<string>();
        commandOptions.Add("checkout");

        if (options != null && options.Length > 0)
            commandOptions.AddRange(options);

        commandOptions.Add(branch);

        _helper.Command(commandOptions.ToArray());
    }

    /// <summary>
    /// Remove files from the working tree and from the index
    /// </summary>
    /// <param name="path">Path of the file(s) to remove</param>
    public void Remove(string path)
    {
        _helper.Command("rm", path);
    }

    /// <summary>
    /// Remove files from the working tree and from the index
    /// </summary>
    /// <param name="path">Path of the file(s) to remove</param>
    /// <param name="options">Options for the rm command</param>
    public void Remove(string path, params string[] options)
    {
        var commandOptions = new List<string>();
        commandOptions.Add("rm");

        if (options != null && options.Length > 0)
            commandOptions.AddRange(options);

        commandOptions.Add(path);

        _helper.Command(commandOptions.ToArray());
    }

    /// <summary>
    /// Creates a tag
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    public void Tag(string tagName)
    {
        _helper.Command("tag", tagName);
    }

    /// <summary>
    /// Create, list, delete or verify a tag
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="options">Options for the tag command</param>
    public void Tag(string tagName, params string[] options)
    {
        var commandOptions = new List<string>();
        commandOptions.Add("tag");

        if (options != null && options.Length > 0)
            commandOptions.AddRange(options);

        commandOptions.Add(tagName);

        _helper.Command(commandOptions.ToArray());
    }

    /// <summary>
    /// Execute any git command
    /// </summary>
    /// <param name="command">The command name like pull or revert</param>
    /// <param name="options">Options for that command</param>
    /// <returns></returns>
    public string? ExecuteCommand(string command, GitCommandOptions? options)
    {
        return _helper.Command(command, options);
    }
}
