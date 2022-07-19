using System.Text.RegularExpressions;

namespace HansPeterGit.Parser;

internal static class CommitParser
{
    private static readonly Regex s_branchAndMessage = new Regex(@"^\[(?<branch>.+) (?<id>.+)\] (?<message>.+)", RegexOptions.Compiled);

    public static Commit Parse(string? output)
    {
        if (string.IsNullOrEmpty(output))
            return new Commit(string.Empty, string.Empty, string.Empty);

        var line = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        if (line == null)
            throw new ArgumentException("Invalid commit output", nameof(output));

        var match = s_branchAndMessage.Match(line);

        if (!match.Success)
            throw new ArgumentException("Invalid commit output", nameof(output));

        return new Commit(match.Groups["id"].Value, match.Groups["branch"].Value, match.Groups["message"].Value.Trim());
    }
}
