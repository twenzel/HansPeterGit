namespace HansPeterGit.Options;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class GitOptionAttribute : Attribute
{
    public string? OptionName { get; }

    public GitOptionAttribute()
    {

    }

    public GitOptionAttribute(string optionName)
    {
        OptionName = optionName;
    }

}