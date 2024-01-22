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
        var branches = repos.GetBranches(false, false);

        branches.Should().NotBeEmpty();
        branches.Should().Contain(b => b.Name == "main");
        branches.Should().Contain(b => b.IsCurrentLocalBranch);
        branches.Should().OnlyContain(b => !b.IsRemote);
    }

    [Test]
    public void GetLocalBranchesWithDetailsTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches(false, true);

        branches.Should().NotBeEmpty();
        branches.Should().Contain(b => b.Name == "main" && b.RemoteBranch == "origin/main" && !string.IsNullOrEmpty(b.CommitHash) && !string.IsNullOrEmpty(b.CommitMessage));
        branches.Should().OnlyContain(b => !b.IsRemote);
    }

    [Test]
    public void GetRemoveBranchesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches(true, false);

        branches.Should().NotBeEmpty();
        branches.Should().Contain(b => b.Name == "origin/main");
        branches.Should().OnlyContain(b => b.IsRemote);
    }

    [Test]
    public void GetRemoveBranchesWithDetailsTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches(true, true);

        branches.Should().NotBeEmpty();
        branches.Should().Contain(b => b.Name == "origin/main" && !string.IsNullOrEmpty(b.CommitHash) && !string.IsNullOrEmpty(b.CommitMessage));
        branches.Should().OnlyContain(b => b.IsRemote);
    }
}
