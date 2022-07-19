namespace HansPeterGit;

public static class UrlHelper
{
    public static string FormatWithCredentials(string url, string username, string password)
    {
        var builder = new UriBuilder(url);
        builder.UserName = username;
        builder.Password = password;
        return builder.Uri.ToString();
    }
}
