using System.Text.RegularExpressions;

namespace HansPeterGit.Parser;

internal static class StatusParser
{
    private static readonly Dictionary<char, FileStatus> s_codeMap = new Dictionary<char, FileStatus>();

    private static readonly Regex s_flagAndPath = new Regex(@"^(?<flag>.) (?<path>.+)$", RegexOptions.Compiled);
    private static readonly Regex s_changedTracked = new Regex(
        @"^1 (?<flag>..) (?<sub>....) (?<mode1>\d{6}) (?<mode2>\d{6}) (?<mode3>\d{6}) (?<name1>\S+) (?<name2>\S+) (?<path>.+)$",
        RegexOptions.Compiled);


    static StatusParser()
    {
        s_codeMap.Add('.', FileStatus.Unchanged);
        s_codeMap.Add('?', FileStatus.Untracked);
        s_codeMap.Add('!', FileStatus.Ignored);
        s_codeMap.Add('M', FileStatus.Modified);
        s_codeMap.Add('A', FileStatus.Added);
    }

    public static RepositoryStatus ParseOutput(string stdout)
    {
        var entries = new List<StatusEntry>();
        var branchName = string.Empty;
        var commit = string.Empty;

        foreach (var line in stdout.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            Match? itemMatch = null;
            switch (line[0])
            {
                case '#':
                    if (line.StartsWith("# branch.oid ", StringComparison.OrdinalIgnoreCase))
                        commit = line.Substring(13);
                    else if (line.StartsWith("# branch.head ", StringComparison.OrdinalIgnoreCase))
                        branchName = line.Substring(14);

                    continue;

                case '1':
                    itemMatch = s_changedTracked.Match(line);
                    break;

                case '!':
                case '?':
                    itemMatch = s_flagAndPath.Match(line);
                    break;

                default:
                    // TODO - parse rename/copy/unknown/etc						
                    break;
            }

            AddEntry(entries, line, itemMatch);
        }

        return new RepositoryStatus(entries, commit, branchName);
    }

    private static void AddEntry(List<StatusEntry> entries, string line, Match? itemMatch)
    {
        if (itemMatch != null)
        {
            if (itemMatch.Success)
            {
                var item = new StatusEntry
                {
                    FilePath = itemMatch.Groups["path"].Value,
                };

                var code = itemMatch.Groups["flag"].Value;
                if (code.Length == 2)
                {
                    item.IndexStatus = s_codeMap[code[0]];
                    item.WorkDirStatus = s_codeMap[code[1]];
                }
                else
                {
                    item.IndexStatus = FileStatus.Unknown;
                    item.WorkDirStatus = s_codeMap[code[0]];
                }

                entries.Add(item);
            }
            else
            {
                throw new InvalidOperationException($"Could not parse item line: {line}");
            }
        }
    }
}
