using System.Reflection;
using HansPeterGit.Options;

namespace HansPeterGit;

/// <summary>
/// Base class for all git command options
/// </summary>
public abstract class GitCommandOptions
{
    private static readonly Dictionary<Type, List<GitCommandOption>> s_properties = new Dictionary<Type, List<GitCommandOption>>();

    /// <summary>
    /// Gets or sets any additional options
    /// </summary>
    public IEnumerable<string>? AdditionalOptions { get; set; }

    /// <summary>
    /// Gets or sets any additional end options
    /// </summary>
    public IEnumerable<string>? AdditionalEndOptions { get; set; }

    /// <summary>
    /// Adds the options from the command options to the list
    /// </summary>
    /// <param name="options">The options instance</param>
    /// <param name="commandOptions">The list to add the options to.</param>
    public static void AddToOptions(GitCommandOptions options, List<string> commandOptions)
    {
        var optionDefinitions = GetOptions(options.GetType());

        if (options.AdditionalOptions != null)
            commandOptions.AddRange(options.AdditionalOptions);

        foreach (var option in optionDefinitions)
        {
            var value = option.GetValue(options);

            if (value != null)
            {
                if (option.IsBoolean && option.OptionName != null)
                {
                    if ((bool)value)
                        commandOptions.Add(option.OptionName);
                }
                else if (option.OptionName != null)
                {
                    commandOptions.Add($"{option.OptionName} {value}");
                }
                else if (option.OptionName == null)
                {
                    commandOptions.Add($"{value}");
                }
            }
        }

        if (options.AdditionalEndOptions != null)
            commandOptions.AddRange(options.AdditionalEndOptions);
    }

    private static List<GitCommandOption> GetOptions(Type optionsType)
    {
        if (!s_properties.TryGetValue(optionsType, out var options))
        {
            var properties = optionsType.GetProperties();
            options = new List<GitCommandOption>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<GitOptionAttribute>();

                if (attribute != null)
                    options.Add(new GitCommandOption(attribute.OptionName, property, GetIsBoolean(property.PropertyType)));
            }

            // ensure options with names comes first
            options = options.OrderByDescending(o => o.OptionName).ThenBy(o => o.PropertyInfo.Name).ToList();

            s_properties.Add(optionsType, options);
        }

        return options;
    }

    private static bool GetIsBoolean(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type == typeof(bool);
    }
}
