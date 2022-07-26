﻿namespace HansPeterGit;

/// <summary>
/// Status of a file within the repository
/// </summary>
[Flags]
public enum FileStatus
{
    /// <summary>
    /// The file hasn't been modified.
    /// </summary>
    Unaltered = 0,

    /// <summary>
    /// New file has been added to the Index. It's unknown from the Head.
    /// </summary>
    NewInIndex = 1,

    /// <summary>
    /// New version of a file has been added to the Index. A previous version exists in the Head.
    /// </summary>
    ModifiedInIndex = 2,

    /// <summary>
    /// The deletion of a file has been promoted from the working directory to the Index. A previous version exists in the Head.
    /// </summary>
    DeletedFromIndex = 4,

    /// <summary>
    /// The renaming of a file has been promoted from the working directory to the Index. A previous version exists in the Head.
    /// </summary>
    RenamedInIndex = 8,

    /// <summary>
    /// A change in type for a file has been promoted from the working directory to the Index. A previous version exists in the Head.
    /// </summary>
    TypeChangeInIndex = 16,

    /// <summary>
    /// New file in the working directory, unknown from the Index and the Head.
    /// </summary>
    NewInWorkdir = 32,

    /// <summary>
    /// The file has been updated in the working directory. A previous version exists in the Index.
    /// </summary>
    ModifiedInWorkdir = 64,

    /// <summary>
    /// The file has been deleted from the working directory. A previous version exists in the Index.
    /// </summary>
    DeletedFromWorkdir = 128,

    /// <summary>
    /// The file type has been changed in the working directory. A previous version exists in the Index.
    /// </summary>
    TypeChangeInWorkdir = 256,

    /// <summary>
    /// The file has been renamed in the working directory.  The previous version at the previous name exists in the Index.
    /// </summary>
    RenamedInWorkdir = 512,

    /// <summary>
    /// The file is unreadable in the working directory.
    /// </summary>
    Unreadable = 1024,

    /// <summary>
    /// The file is <see cref="NewInWorkdir"/> but its name and/or path matches an exclude pattern in a <c>gitignore</c> file.
    /// </summary>
    Ignored = 2048,

    /// <summary>
    /// The file is <see cref="Conflicted"/> due to a merge.
    /// </summary>
    Conflicted = 4096,

    /// <summary>
    /// The file doesn't exist.
    /// </summary>
    Nonexistent = 1073741824,
}
