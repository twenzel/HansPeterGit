using System.Reflection;

namespace HansPeterGit.Options;
internal record GitCommandOption(string? OptionName, PropertyInfo PropertyInfo, bool IsBoolean)
{
    internal object? GetValue(GitCommandOptions options)
    {
        return PropertyInfo.GetValue(options);
    }
}
