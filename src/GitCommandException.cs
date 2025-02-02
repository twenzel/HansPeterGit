﻿using System.Diagnostics;

namespace HansPeterGit;

/// <summary>
/// Git exception when executing a command
/// </summary>
public class GitCommandException : Exception
{
    /// <summary>
    /// Gets the used process
    /// </summary>
    public Process? Process { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="GitCommandException"/> class
    /// </summary>
    /// <param name="message"></param>
    /// <param name="process"></param>
    public GitCommandException(string message, Process process) : base(message)
    {
        Process = process;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="GitCommandException"/> class
    /// </summary>
    public GitCommandException()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="GitCommandException"/> class
    /// </summary>
    public GitCommandException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="GitCommandException"/> class
    /// </summary>
    public GitCommandException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
