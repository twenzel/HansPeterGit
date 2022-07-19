using System.Diagnostics;
using System.Runtime.Serialization;

namespace HansPeterGit;

[Serializable]
public class GitCommandException : Exception
{
    public Process? Process { get; }

    public GitCommandException(string message, Process process) : base(message)
    {
        Process = process;
    }

    public GitCommandException()
    {
    }

    protected GitCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public GitCommandException(string? message) : base(message)
    {
    }

    public GitCommandException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
