using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace HansPeterGit;

/// <summary>
/// Helper class to start git processes
/// </summary>
public partial class GitHelper
{
    private static readonly Regex s_validCommandName = ValidCommandNameExpression();
    private static readonly List<string> s_commandRequiringAuthentication = ["clone", "push"];

    /// <summary>
    /// Starting with version 1.7.10, Git uses UTF-8.
    /// Use this encoding for Git input and output.
    /// </summary>
    private static readonly Encoding s_encoding = new UTF8Encoding(false, true);

    private readonly Stopwatch _stopwatch = new();

    /// <summary>
    /// Gets the current working directory of the Git process.
    /// </summary>
    public string WorkingDirectory => Options.WorkingDirectory;

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    public ILogger? Logger => Options.Logger;

    /// <summary>
    /// Gets the options.
    /// </summary>
    public GitOptions Options { get; }

    /// <summary>
    /// Creates a new instance of the helper
    /// </summary>
    /// <param name="options">The options</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GitHelper(GitOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Runs the given git command, and returns the contents of its STDOUT.
    /// </summary>
    /// <param name="command">The git command</param>
    /// <param name="options">Any command options</param>
    public string? Command(string command, GitCommandOptions? options)
    {
        List<string> commandOptions = [command];

        if (options != null)
            GitCommandOptions.AddToOptions(options, commandOptions);

        return Command(commandOptions);
    }

    /// <summary>
    /// Runs the given git command, and returns the contents of its STDOUT.
    /// </summary>
    public string? Command(params IEnumerable<string> commands)
    {
        string? output = null;
        CommandOutputPipe(stdout => output = stdout.ReadToEnd(), commands);

        if (!string.IsNullOrEmpty(output))
            Logger?.LogDebug("Output: {Output}", output);

        return output;
    }

    /// <summary>
    /// Runs the given git command, and returns the first line of its STDOUT.
    /// </summary>
    public string? CommandOneline(params IEnumerable<string> commands)
    {
        string? output = null;
        CommandOutputPipe(stdout => output = stdout.ReadLine(), commands);

        if (!string.IsNullOrEmpty(output))
            Logger?.LogDebug("Output: {Output}", output);

        return output;
    }

    /// <summary>
    /// Runs the given git command, and redirects STDOUT to the provided action.
    /// </summary>
    public void CommandOutputPipe(Action<TextReader> handleOutput, params IEnumerable<string> commands)
    {
        MeasureTime(commands, () =>
        {
            AssertValidCommand(commands);
            var process = Start(commands, RedirectStdout);
            handleOutput(process.StandardOutput);
            Close(process);
        });
    }

    /// <summary>
    /// Runs the given git command and provides a text writer to define console input
    /// </summary>
    public void CommandInputPipe(Action<TextWriter> action, params IEnumerable<string> commands)
    {
        MeasureTime(commands, () =>
        {
            AssertValidCommand(commands);
            var process = Start(commands, RedirectStdin);
            action(process.StandardInput.WithEncoding(s_encoding));
            Close(process);
        });
    }

    /// <summary>
    /// Runs the given git command and provides a text writer to define console input and a reader to read the console
    /// </summary>
    public void CommandInputOutputPipe(Action<TextWriter, TextReader> interact, params IEnumerable<string> commands)
    {
        MeasureTime(commands, () =>
        {
            AssertValidCommand(commands);
            var process = Start(commands, Extensions.And<ProcessStartInfo>(RedirectStdin, RedirectStdout));
            interact(process.StandardInput.WithEncoding(s_encoding), process.StandardOutput);
            Close(process);
        });
    }

    private void MeasureTime(IEnumerable<string> commands, Action action)
    {
        if (!Options.LogGitCommandDuration)
            action();
        else
        {
            _stopwatch.Restart();
            try
            {
                action();
            }
            finally
            {
                _stopwatch.Stop();
                Logger?.LogDebug("Git command time [{Duration}ms] {Command}", _stopwatch.ElapsedMilliseconds, string.Join(" ", commands));
            }
        }
    }

    private void Close(GitProcess process)
    {
        // if caller doesn't read entire stdout to the EOF - it is possible that
        // child process will hang waiting until there will be free space in stdout
        // buffer to write the rest of the output.
        // See https://github.com/git-tfs/git-tfs/issues/121 for details.
        if (process.StartInfo.RedirectStandardOutput)
        {
            process.StandardOutput.BaseStream.CopyTo(Stream.Null);
            process.StandardOutput.Close();
        }

        if (!string.IsNullOrEmpty(process.StandardErrorString))
            Logger?.LogDebug("StdErr: {StandardError}", process.StandardErrorString);

        if (!process.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds))
            throw new GitCommandException("Command did not terminate.", process);
        if (process.ExitCode != 0)
            throw new GitCommandException(string.Format("Command exited with error code: {0}\n{1}", process.ExitCode, process.StandardErrorString), process);
    }

    private static void RedirectStdout(ProcessStartInfo startInfo)
    {
        startInfo.RedirectStandardOutput = true;
        startInfo.StandardOutputEncoding = s_encoding;
    }

    private static void RedirectStderr(ProcessStartInfo startInfo)
    {
        startInfo.RedirectStandardError = true;
        startInfo.StandardErrorEncoding = s_encoding;
    }

    private static void RedirectStdin(ProcessStartInfo startInfo)
    {
        startInfo.RedirectStandardInput = true;
        // there is no StandardInputEncoding property, use extension method StreamWriter.WithEncoding instead
    }

    private GitProcess Start(IEnumerable<string> commands, Action<ProcessStartInfo> initialize)
    {
        var startInfo = new ProcessStartInfo();
        startInfo.FileName = Options.PathToGit;
        startInfo.SetArguments(commands);
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.EnvironmentVariables["GIT_PAGER"] = "cat";

        Options.SetEnvironmentVariables?.Invoke(startInfo.EnvironmentVariables);

        if (!string.IsNullOrEmpty(WorkingDirectory))
            startInfo.WorkingDirectory = WorkingDirectory;

        if (IsAuthenticationRequired(commands))
            AddAuthentication(startInfo);

        RedirectStderr(startInfo);
        initialize(startInfo);
        Logger?.LogDebug("Starting process: {Filename} {Arguments}", startInfo.FileName, Helper.MaskCredentials(startInfo.Arguments));

        var sysProcess = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start process: " + startInfo.FileName + " " + Helper.MaskCredentials(startInfo.Arguments));

        var process = new GitProcess(sysProcess, Logger);
        process.ConsumeStandardError();
        return process;
    }

    private void AddAuthentication(ProcessStartInfo startInfo)
    {
        Options.Authentication?.AddAuthentication(startInfo);
    }

    private static bool IsAuthenticationRequired(IEnumerable<string> commands)
    {
        return (commands.Any() && s_commandRequiringAuthentication.Contains(commands.First(), StringComparer.OrdinalIgnoreCase));
    }

    private static void AssertValidCommand(IEnumerable<string> commands)
    {
        var commandsExists = commands.Any();
        if (!commandsExists || !s_validCommandName.IsMatch(commands.First()))
            throw new InvalidOperationException("bad git command: " + (!commandsExists ? "" : commands.First()));
    }

    internal class GitProcess
    {
        private readonly Process _process;

        public string? StandardErrorString { get; private set; }

        public ProcessStartInfo StartInfo { get { return _process.StartInfo; } }
        public int ExitCode { get { return _process.ExitCode; } }
        public StreamWriter StandardInput { get { return _process.StandardInput; } }
        public StreamReader StandardOutput { get { return _process.StandardOutput; } }

        public ILogger? Logger { get; }

        public GitProcess(Process process, ILogger? logger)
        {
            _process = process;
            Logger = logger;
        }

        public static implicit operator Process(GitProcess process)
        {
            return process._process;
        }

        public void ConsumeStandardError()
        {
            StandardErrorString = "";
            _process.ErrorDataReceived += StdErrReceived;
            _process.BeginErrorReadLine();
        }

        private void StdErrReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Trim() != "")
            {
                var data = e.Data;
                Logger?.LogInformation("Git error: {Error}", data.TrimEnd());
                StandardErrorString += data;
            }
        }

        public void WaitForExit()
        {
            _process.WaitForExit();
        }

        public bool WaitForExit(int milliseconds)
        {
            return _process.WaitForExit(milliseconds);
        }
    }

    [GeneratedRegex("^[a-z0-9A-Z_-]+$")]
    private static partial Regex ValidCommandNameExpression();
}
