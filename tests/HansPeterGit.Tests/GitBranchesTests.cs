using NUnit.Framework;

namespace HansPeterGit.Tests;

[TestFixture]
public class GitBranchesTests
{
    [Test]
    public void GetBranchesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches();

        var branchesArray = branches as string[] ?? branches.ToArray();

        Assert.That(branchesArray, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(branchesArray.Any(branch => branch == string.Empty), Is.False);
            Assert.That(branchesArray, Contains.Item("main"));
        });
    }

    [Test]
    public void GetRemoteBranchesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches(true);

        var branchesArray = branches as string[] ?? branches.ToArray();

        Assert.That(branchesArray, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(branchesArray.Any(branch => branch == string.Empty), Is.False);
            Assert.That(branchesArray.Any(branch => branch.Contains("origin/HEAD")), Is.True);
        });
    }

    [Test]
    public void GetLocalBranchesTest()
    {
        var repos = new GitRepository(".");
        var branches = repos.GetBranches(true);

        var branchesArray = branches as string[] ?? branches.ToArray();

        Assert.That(branchesArray, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(branchesArray.Any(branch => branch == string.Empty), Is.False);
            Assert.That(branchesArray.Any(branch => branch.Contains("origin/HEAD")), Is.True);
        });
    }
}