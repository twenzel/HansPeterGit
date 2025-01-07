using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace HansPeterGit;

/// <summary>
/// Contains information about the repository status
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RepositoryStatus : IEnumerable<StatusEntry>
{
    private readonly ICollection<StatusEntry> _statusEntries;
    private readonly List<StatusEntry> _added = [];
    private readonly List<StatusEntry> _staged = [];
    private readonly List<StatusEntry> _removed = [];
    private readonly List<StatusEntry> _missing = [];
    private readonly List<StatusEntry> _modified = [];
    private readonly List<StatusEntry> _untracked = [];
    private readonly List<StatusEntry> _ignored = [];
    private readonly List<StatusEntry> _renamedInIndex = [];
    private readonly List<StatusEntry> _renamedInWorkDir = [];
    private readonly List<StatusEntry> _unchanged = [];

    private static readonly Dictionary<FileStatus, Action<RepositoryStatus, StatusEntry>> s_mapper = CreateMapper();

    /// <summary>
    /// Gets whether the current status is dirty (aka changes exists)
    /// </summary>
    public bool IsDirty { get; }

    /// <summary>
    /// Gets the current branch
    /// </summary>
    public string Branch { get; }

    /// <summary>
    /// Gets the current commit id
    /// </summary>
    public string Commit { get; }

    /// <summary>
    /// Gets the status entry by the given path
    /// </summary>
    /// <param name="path">Path to retrieve the entry</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public StatusEntry this[string path]
    {
        get
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var list = _statusEntries.Where((StatusEntry e) => string.Equals(e.FilePath, path, StringComparison.Ordinal)).ToList();
            if (list.Count == 0)
            {
                return new StatusEntry { FilePath = path, State = FileStatus.Nonexistent };
            }

            return list.Single();
        }
    }

    /// <summary>
    /// List of files added to the index, which are not in the current commit
    /// </summary>
    public IEnumerable<StatusEntry> Added => _added;

    /// <summary>
    /// List of files added to the index, which are already in the current commit with different content
    /// </summary>
    public IEnumerable<StatusEntry> Staged => _staged;

    /// <summary>
    /// List of files removed from the index but are existent in the current commit
    /// </summary>
    public IEnumerable<StatusEntry> Removed => _removed;

    /// <summary>
    /// List of files existent in the index but are missing in the working directory
    /// </summary>
    public IEnumerable<StatusEntry> Missing => _missing;

    /// <summary>
    /// List of files with unstaged modifications. A file may be modified and staged at the same time if it has been modified after adding.
    /// </summary>
    public IEnumerable<StatusEntry> Modified => _modified;

    /// <summary>
    /// List of files existing in the working directory but are neither tracked in the index nor in the current commit.
    /// </summary>
    public IEnumerable<StatusEntry> Untracked => _untracked;

    /// <summary>
    /// List of files existing in the working directory that are ignored.
    /// </summary>
    public IEnumerable<StatusEntry> Ignored => _ignored;

    /// <summary>
    /// List of files that were renamed and staged.
    /// </summary>
    public IEnumerable<StatusEntry> RenamedInIndex => _renamedInIndex;

    /// <summary>
    /// List of files that were renamed in the working directory but have not been staged.
    /// </summary>
    public IEnumerable<StatusEntry> RenamedInWorkDir => _renamedInWorkDir;

    /// <summary>
    /// List of files that were unmodified in the working directory.
    /// </summary>
    public IEnumerable<StatusEntry> Unchanged => _unchanged;


    internal RepositoryStatus(IEnumerable<StatusEntry> entries, string commit, string branch)
    {
        _statusEntries = [];

        BuildLists(entries);

        IsDirty = _statusEntries.Any((StatusEntry entry) => entry.State != FileStatus.Ignored && entry.State != FileStatus.Unaltered);
        Commit = commit;
        Branch = branch;
    }

    private void BuildLists(IEnumerable<StatusEntry> entries)
    {
        foreach (var entry in entries)
            AddEntry(entry);
    }

    private void AddEntry(StatusEntry entry)
    {
        _statusEntries.Add(entry);

        if (entry.State == FileStatus.Unaltered)
        {
            _unchanged.Add(entry);
        }
        else
        {
            foreach (var info in s_mapper)
            {
                if (!entry.State.HasFlag(info.Key))
                    continue;

                info.Value(this, entry);
            }
        }
    }

    private static Dictionary<FileStatus, Action<RepositoryStatus, StatusEntry>> CreateMapper()
    {
        return new Dictionary<FileStatus, Action<RepositoryStatus, StatusEntry>>
        {
            { FileStatus.NewInWorkdir, (rs, s) => rs._untracked.Add(s) },
            { FileStatus.ModifiedInWorkdir, (rs, s) => rs._modified.Add(s) },
            { FileStatus.DeletedFromWorkdir, (rs, s) => rs._missing.Add(s) },
            { FileStatus.NewInIndex, (rs, s) => rs._added.Add(s) },
            { FileStatus.ModifiedInIndex, (rs, s) => rs._staged.Add(s) },
            { FileStatus.DeletedFromIndex, (rs, s) => rs._removed.Add(s) },
            { FileStatus.RenamedInIndex, (rs, s) => rs._renamedInIndex.Add(s) },
            { FileStatus.Ignored, (rs, s) => rs._ignored.Add(s) },
            { FileStatus.RenamedInWorkdir, (rs, s) => rs._renamedInWorkDir.Add(s) },
            { FileStatus.Conflicted, (rs, s) => rs._renamedInWorkDir.Add(s) },
         };
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns></returns>		
    public IEnumerator<StatusEntry> GetEnumerator() => _statusEntries.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private string DebuggerDisplay => string.Format(CultureInfo.InvariantCulture, "+{0} ~{1} -{2} | +{3} ~{4} -{5} | i{6}", Added.Count(), Staged.Count(), Removed.Count(), Untracked.Count(), Modified.Count(), Missing.Count(), Ignored.Count());

}
