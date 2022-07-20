using System.Text.RegularExpressions;

namespace HansPeterGit;

/// <summary>
/// Little helper class
/// </summary>
public static class Helper
{
    private static readonly Regex s_credentials = new Regex("\"AUTHORIZATION:(?<authType>.*) (?<credential>.*)\"", RegexOptions.Compiled);

    /// <summary>
    /// Adds the username and password to the url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string FormatWithCredentials(string url, string username, string password)
    {
        var builder = new UriBuilder(url);
        builder.UserName = username;
        builder.Password = password;
        return builder.Uri.AbsoluteUri;
    }

    /// <summary>
    /// Deletes a directory recursive.
    /// </summary>
    /// <remarks>
    /// Git (and libgit2, and thus LibGit2Sharp) create object files without write permission, and this is by-design. As a consequence, you must make them writable before removing them.
    /// </remarks>
    /// <param name="directory">The directory to delete with sub folders</param>
    public static void DeleteDirectory(string directory)
    {
        if (!Directory.Exists(directory))
            return;

        foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            DeleteDirectory(subdirectory);

        foreach (var fileName in Directory.EnumerateFiles(directory))
        {
            var fileInfo = new FileInfo(fileName)
            {
                Attributes = FileAttributes.Normal
            };
            fileInfo.Delete();
        }

        Directory.Delete(directory, true);
    }

    /// <summary>
    /// Masks any credentials in the given arguments
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static string MaskCredentials(string arguments)
    {
        var match = s_credentials.Match(arguments);

        if (match.Success)
        {
            var credential = match.Groups["credential"].Value;
            var masked = new string('*', credential.Length);
            return arguments.Replace(credential, masked);
        }

        return arguments;
    }
}
