using FluentAssertions;
using HansPeterGit.Options;
using NUnit.Framework;

namespace HansPeterGit.Tests;
public class GitCommandOptionsTests
{
    public class AddToOptionsMethod : GitCommandOptionsTests
    {
        [Test]
        public void Add_Options_To_List()
        {
            var options = new TestOptions();
            options.Remote = "someRemote";
            options.All = true;
            options.Prune = true;
            options.Deep = 5;
            options.ShallowExclude = "abc";
            options.AdditionalOptions = new List<string> { "--test", "--bb=5" };
            options.AdditionalEndOptions = new List<string> { "group" };

            var commands = new List<string>();

            GitCommandOptions.AddToOptions(options, commands);
            commands.Should().HaveCount(8);
            commands.Should().Contain("someRemote");
            commands.Should().Contain("--all");
            commands.Should().Contain("--prune");
            commands.Should().Contain("--depth=5");
            commands.Should().Contain("--shallow-exclude=abc");
            commands.Should().Contain("--bb=5");
            commands.Should().Contain("group");
        }

        [Test]
        public void Add_Options_To_List_In_Correct_Order()
        {
            var options = new TestOptions();
            options.Remote = "someRemote";
            options.All = true;
            options.Prune = true;
            options.Deep = 5;
            options.ShallowExclude = "abc";
            options.AdditionalOptions = new List<string> { "--test", "--bb=5" };
            options.AdditionalEndOptions = new List<string> { "group" };

            var commands = new List<string>();

            GitCommandOptions.AddToOptions(options, commands);
            var all = string.Join(" ", commands);

            all.Should().Be("--test --bb=5 --shallow-exclude=abc --prune --depth=5 --all someRemote group");
        }
    }

    public class TestOptions : GitCommandOptions
    {
        [GitOption]
        public string? Remote { get; set; }

        [GitOption("--prune")]
        public bool? Prune { get; set; }

        [GitOption("--all")]
        public bool? All { get; set; }

        [GitOption("--depth")]
        public int? Deep { get; set; }

        [GitOption("--shallow-exclude")]
        public string? ShallowExclude { get; set; }
    }
}
