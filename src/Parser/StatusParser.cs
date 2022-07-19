using System.Text.RegularExpressions;

namespace HansPeterGit.Parser;

internal static class StatusParser
{
    private static readonly Dictionary<char, InternalFileStatus> s_codeMap = new Dictionary<char, InternalFileStatus>();

    private static readonly Regex s_flagAndPath = new Regex(@"^(?<flag>.) (?<path>.+)$", RegexOptions.Compiled);
    private static readonly Regex s_changedTracked = new Regex(
        @"^1 (?<flag>..) (?<sub>....) (?<mode1>\d{6}) (?<mode2>\d{6}) (?<mode3>\d{6}) (?<name1>\S+) (?<name2>\S+) (?<path>.+)$",
        RegexOptions.Compiled);
    private static readonly Regex s_unmerged = new Regex(
    @"^u (?<flag>..) (?<sub>....) (?<mode1>\d{6}) (?<mode2>\d{6}) (?<mode3>\d{6}) (?<modeW>\d{6}) (?<name1>\S+) (?<name2>\S+) (?<name3>\S+) (?<path>.+)$",
    RegexOptions.Compiled);

    private enum InternalFileStatus
    {
        Unmodified,
        Modified,
        FileTypeChange, // (regular file, symbolic link or submodule)
        Added,
        Deleted,
        Renamed,
        Copied,
        Updated, // updated but unmerged
        Untracked,
        Ignored,
        Unknown
    }

    static StatusParser()
    {
        s_codeMap.Add('.', InternalFileStatus.Unmodified);
        s_codeMap.Add('M', InternalFileStatus.Modified);
        s_codeMap.Add('T', InternalFileStatus.FileTypeChange);
        s_codeMap.Add('A', InternalFileStatus.Added);
        s_codeMap.Add('D', InternalFileStatus.Deleted);
        s_codeMap.Add('R', InternalFileStatus.Renamed);
        s_codeMap.Add('C', InternalFileStatus.Copied);
        s_codeMap.Add('U', InternalFileStatus.Updated);
        s_codeMap.Add('?', InternalFileStatus.Untracked);
        s_codeMap.Add('!', InternalFileStatus.Ignored);
    }

    public static RepositoryStatus Parse(string output)
    {
        var entries = new List<StatusEntry>();
        var branchName = string.Empty;
        var commit = string.Empty;

        foreach (var line in output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
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

                case 'u':
                    itemMatch = s_unmerged.Match(line);
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
                var indexStatus = InternalFileStatus.Unknown;
                InternalFileStatus workdirStatus;

                var code = itemMatch.Groups["flag"].Value;
                if (code.Length == 2)
                {
                    indexStatus = s_codeMap[code[0]];
                    workdirStatus = s_codeMap[code[1]];
                }
                else
                {
                    workdirStatus = s_codeMap[code[0]];
                }

                var item = new StatusEntry
                {
                    FilePath = itemMatch.Groups["path"].Value,
                    State = CalculateState(indexStatus, workdirStatus)
                };

                entries.Add(item);
            }
            else
            {
                throw new InvalidOperationException($"Could not parse item line: {line}");
            }
        }
    }
    private static FileStatus CalculateState(InternalFileStatus indexStatus, InternalFileStatus workdirStatus)
    {
        return (indexStatus, workdirStatus) switch
        {
            (InternalFileStatus.Unknown, InternalFileStatus.Untracked) => FileStatus.NewInWorkdir,
            (InternalFileStatus.Unknown, InternalFileStatus.Ignored) => FileStatus.Ignored,

            (InternalFileStatus.Unmodified, InternalFileStatus.Renamed) => FileStatus.RenamedInWorkdir,
            (InternalFileStatus.Unmodified, InternalFileStatus.Copied) => FileStatus.NewInWorkdir,
            (InternalFileStatus.Unmodified, InternalFileStatus.Modified) => FileStatus.ModifiedInWorkdir,
            (InternalFileStatus.Unmodified, InternalFileStatus.Added) => FileStatus.NewInWorkdir,
            (InternalFileStatus.Unmodified, InternalFileStatus.Deleted) => FileStatus.DeletedFromWorkdir,
            (InternalFileStatus.Unmodified, _) => FileStatus.Unaltered,
            (InternalFileStatus.Modified, _) => FileStatus.ModifiedInIndex,
            (InternalFileStatus.Added, InternalFileStatus.Unmodified) => FileStatus.NewInIndex,
            (InternalFileStatus.Deleted, InternalFileStatus.Unmodified) => FileStatus.DeletedFromIndex,
            (InternalFileStatus.FileTypeChange, _) => FileStatus.TypeChangeInIndex,
            (_, InternalFileStatus.FileTypeChange) => FileStatus.TypeChangeInWorkdir,

            (InternalFileStatus.Updated, _) => FileStatus.Conflicted,
            (_, InternalFileStatus.Updated) => FileStatus.Conflicted,
            (InternalFileStatus.Deleted, InternalFileStatus.Deleted) => FileStatus.Conflicted,
            (InternalFileStatus.Added, InternalFileStatus.Added) => FileStatus.Conflicted,

            _ => FileStatus.Unaltered,
        };

    }
}
