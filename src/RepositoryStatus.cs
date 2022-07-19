using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace HansPeterGit;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RepositoryStatus : IEnumerable<StatusEntry>
{
    private readonly ICollection<StatusEntry> _statusEntries;
    private readonly List<StatusEntry> _added = new List<StatusEntry>();
    private readonly List<StatusEntry> _staged = new List<StatusEntry>();
    private readonly List<StatusEntry> _removed = new List<StatusEntry>();
    private readonly List<StatusEntry> _missing = new List<StatusEntry>();
    private readonly List<StatusEntry> _modified = new List<StatusEntry>();
    private readonly List<StatusEntry> _untracked = new List<StatusEntry>();
    private readonly List<StatusEntry> _ignored = new List<StatusEntry>();
    private readonly List<StatusEntry> _renamed = new List<StatusEntry>();
    private readonly List<StatusEntry> _renamedInWorkDir = new List<StatusEntry>();
    private readonly List<StatusEntry> _unchanged = new List<StatusEntry>();
    private readonly List<StatusEntry> _typeChanged = new List<StatusEntry>();

    private static readonly Dictionary<FileStatus, Action<RepositoryStatus, StatusEntry>> s_mapper = CreateMapper();

    public bool IsDirty { get; }

    /// <summary>
    /// Gets the current branch
    /// </summary>
    public string Branch { get; }

    /// <summary>
    /// Gets the current commit id
    /// </summary>
    public string Commit { get; }

    public StatusEntry this[string path]
    {
        get
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var list = _statusEntries.Where((StatusEntry e) => string.Equals(e.FilePath, path, StringComparison.Ordinal)).ToList();
            if (list.Count == 0)
            {
                return new StatusEntry { FilePath = path, WorkDirStatus = FileStatus.Nonexistent, IndexStatus = FileStatus.Nonexistent };
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
    public IEnumerable<StatusEntry> RenamedInIndex => _renamed;

    /// <summary>
    /// List of files that were renamed in the working directory but have not been staged.
    /// </summary>
    public IEnumerable<StatusEntry> RenamedInWorkDir => _renamedInWorkDir;

    /// <summary>
    /// List of files that were unmodified in the working directory.
    /// </summary>
    public IEnumerable<StatusEntry> Unchanged => _unchanged;

    /// <summary>
    /// List of files that were changed by its type.
    /// </summary>
    public IEnumerable<StatusEntry> TypeChanged => _typeChanged;

    internal RepositoryStatus(IEnumerable<StatusEntry> entries, string commit, string branch)
    {
        _statusEntries = new List<StatusEntry>();

        BuildLists(entries);

        IsDirty = _statusEntries.Any((StatusEntry entry) =>
            (entry.WorkDirStatus != FileStatus.Ignored && entry.WorkDirStatus != FileStatus.Unchanged)
            || (entry.IndexStatus != FileStatus.Ignored && entry.IndexStatus != FileStatus.Unchanged)
            );
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
        s_mapper[entry.WorkDirStatus](this, entry);
    }

    private static Dictionary<FileStatus, Action<RepositoryStatus, StatusEntry>> CreateMapper()
    {
        return new Dictionary<FileStatus, Action<RepositoryStatus, StatusEntry>>
            {
				//{ FileStatus.NewInWorkdir, (status, entry) => status._added.Add(entry) },
				//{ FileStatus.ModifiedInWorkdir, (status, entry) => status._modified.Add(entry) },
				//{ FileStatus.DeletedFromWorkdir, (status, entry) => status._removed.Add(entry) },
				//{ FileStatus.NewInIndex, (status, entry) => status._staged.Add(entry) },
				//{ FileStatus.ModifiedInIndex, (status, entry) => status._staged.Add(entry) },
				//{ FileStatus.DeletedFromIndex, (status, entry) => status._removed.Add(entry) },
				//{ FileStatus.Missing, (status, entry) => status._missing.Add(entry) },
				//{ FileStatus.Unmerged, (status, entry) => status._modified.Add(entry) },
				//{ FileStatus.Ignored, (status, entry) => status._ignored.Add(entry) },
				//{ FileStatus.RenamedInIndex, (status, entry) => status._renamedInIndex.Add(entry) },
				//{ FileStatus.RenamedInWorkdir, (status, entry) => status._renamedInWorkDir.Add(entry) },
				//{ FileStatus.Unaltered, (status, entry) => status._unaltered.Add(entry) }
				{ FileStatus.Added, (status, entry) => status._added.Add(entry) },
                { FileStatus.Ignored, (status, entry) => status._ignored.Add(entry) },
                { FileStatus.Modified, (status, entry) => status._modified.Add(entry) },
                { FileStatus.Removed, (status, entry) => status._removed.Add(entry) },
                { FileStatus.Renamed, (status, entry) => status._renamed.Add(entry) },
                { FileStatus.TypeChange, (status, entry) => status._typeChanged.Add(entry) },
                { FileStatus.Unchanged, (status, entry) => status._unchanged.Add(entry) },
                { FileStatus.Untracked, (status, entry) => status._untracked.Add(entry) },
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
