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
            var options = new FetchOptions();
            options.Remote = "someRemote";
            options.All = true;
            options.Prune = true;
            options.PruneTags = true;
            options.AdditionalOptions = new List<string> { "--test", "--bb=5" };
            options.AdditionalEndOptions = new List<string> { "group" };

            var commands = new List<string>();

            GitCommandOptions.AddToOptions(options, commands);
            commands.Should().HaveCount(7);
            commands.Should().Contain("someRemote");
            commands.Should().Contain("--all");
            commands.Should().Contain("--prune");
            commands.Should().Contain("--prune-tags");
            commands.Should().Contain("--test");
            commands.Should().Contain("--bb=5");
            commands.Should().Contain("group");
        }

        [Test]
        public void Add_Options_To_List_In_Correct_Order()
        {
            var options = new FetchOptions();
            options.Remote = "someRemote";
            options.All = true;
            options.Prune = true;
            options.PruneTags = true;
            options.AdditionalOptions = new List<string> { "--test", "--bb=5" };
            options.AdditionalEndOptions = new List<string> { "group" };

            var commands = new List<string>();

            GitCommandOptions.AddToOptions(options, commands);
            var all = string.Join(" ", commands);

            all.Should().Be("--test --bb=5 --prune-tags --prune --all someRemote group");
        }
    }
}
