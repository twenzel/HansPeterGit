using System.Diagnostics;
using System.Text;

namespace HansPeterGit;

/// <summary>
/// Internal extensions
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// The encoding used by a stream is a read-only property. Use this method to
    /// create a new stream based on <paramref name="stream"/> that uses
    /// the given <paramref name="encoding"/> instead.
    /// </summary>
    public static StreamWriter WithEncoding(this StreamWriter stream, Encoding encoding)
    {
        return new StreamWriter(stream.BaseStream, encoding);
    }

    public static Action<T> And<T>(this Action<T> originalAction, params Action<T>[] additionalActions)
    {
        return x =>
        {
            originalAction(x);
            foreach (var action in additionalActions)
            {
                action(x);
            }
        };
    }

    public static void SetArguments(this ProcessStartInfo startInfo, params string[] args)
    {
        startInfo.Arguments = string.Join(" ", args.Select(arg => QuoteProcessArgument(arg)).ToArray());
    }

    private static string QuoteProcessArgument(string arg)
    {
        return (arg.Contains(' ') && !arg.Contains('=')) ? $"\"{arg}\"" : arg;
    }
}
