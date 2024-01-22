using FluentAssertions;
using NUnit.Framework;

namespace HansPeterGit.Tests;

public class GitBranchesTests
{
    [Test]
    public void GetRemoteBranchNamesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranchNames(true);

        branches.Should().NotBeEmpty();
        branches.Should().NotContain(branch => branch == string.Empty);
        branches.Should().Contain("origin/main");
    }

    [Test]
    public void GetLocalBranchNamesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranchNames(false);

        branches.Should().NotBeEmpty();
        branches.Should().NotContain(branch => branch == string.Empty);
        branches.Should().Contain("main");
    }


    [Test]
    public void GetLocalBranchesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches(false);

        branches.Should().NotBeEmpty();
        branches.Should().Contain(b => b.Name == "main" && b.IsCurrentLocalBranch);
        branches.Should().OnlyContain(b => !b.IsRemote);
    }

    [Test]
    public void GetRemoveBranchesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches(true);

        branches.Should().NotBeEmpty();
        branches.Should().Contain(b => b.Name == "origin/main");
        branches.Should().OnlyContain(b => b.IsRemote);
    }
}
