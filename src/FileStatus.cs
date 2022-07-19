namespace HansPeterGit;

public enum FileStatus
{
    Unknown,

    /// <summary>
    /// Files added to the index, which are not in the current commit
    /// </summary>
    Added,

    /// <summary>
    /// Files with unstaged modifications. A file may be modified and staged at the same time if it has been modified after adding.
    /// </summary>
    Modified,
    /// <summary>
    /// Files removed from the index but are existent in the current commit
    /// </summary>
    Removed,
    Renamed,
    TypeChange,

    /// <summary>
    /// Files existing in the working directory that are ignored.
    /// </summary>
    Ignored,
    Unchanged,
    Nonexistent,

    /// <summary>
    /// Files existing in the working directory but are neither tracked in the index nor in the current commit.
    /// </summary>
    Untracked
}



/////List of files added to the index, which are already in the current commit with    different content
//Staged



//	/// List of files existent in the index but are missing in the working directory
//	Missing



//	///List of files existing in the working directory but are neither tracked in the index nor in the current commit.
//	Untracked


//	/// List of files that were renamed and staged.
//	RenamedInIndex

//	///List of files that were renamed in the working directory but have not been staged.
//	RenamedInWorkDir

//	///List of files that were unmodified in the working directory.
//	Unaltered
