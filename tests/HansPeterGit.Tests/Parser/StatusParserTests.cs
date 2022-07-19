using System.Reflection;
using FluentAssertions;
using HansPeterGit.Parser;
using NUnit.Framework;

namespace HansPeterGit.Tests.Parser;
public class StatusParserTests
{
    [Test]
    public void Can_Parse_Header()
    {
        var status = StatusParser.Parse("# branch.oid a7f4ae023f4ee62b4a31292008ca39b782d0d516\n# branch.head master\n1 A. N... 000000 100644 100644 0000000000000000000000000000000000000000 88c3b23cbb80de47d9c9ceb3e837e273be734a1f sub/test.json");
        status.Branch.Should().Be("master");
        status.Commit.Should().Be("a7f4ae023f4ee62b4a31292008ca39b782d0d516");
    }

    [Test]
    public void Can_Parse_Status()
    {
        var output = GetResourceString("GitStatusSample.txt");

        var status = StatusParser.Parse(output);
        status.Branch.Should().Be("master");
        status.Commit.Should().Be("a7f4ae023f4ee62b4a31292008ca39b782d0d516");

        status.Should().Contain(i => i.FilePath == "staged_new.json" && i.State == FileStatus.NewInIndex);
        status.Should().Contain(i => i.FilePath == "notstaged_modified.json" && i.State == FileStatus.ModifiedInWorkdir);
        status.Should().Contain(i => i.FilePath == "staged_modified.json" && i.State == FileStatus.ModifiedInIndex);
        status.Should().Contain(i => i.FilePath == "notstaged_deleted.json" && i.State == FileStatus.DeletedFromWorkdir);
        status.Should().Contain(i => i.FilePath == "staged_deleted.json" && i.State == FileStatus.DeletedFromIndex);
        status.Should().Contain(i => i.FilePath == "someuntracked.json" && i.State == FileStatus.NewInWorkdir);
        status.Should().Contain(i => i.FilePath == "someignored.txt" && i.State == FileStatus.Ignored);
        status.Should().Contain(i => i.FilePath == "unmerged_both_modified.md" && i.State == FileStatus.Conflicted);
    }

    [Test]
    public void Status_Lists_Are_Filled()
    {
        var output = GetResourceString("GitStatusSample.txt");

        var status = StatusParser.Parse(output);

        status.Added.Should().HaveCount(1);
        status.Added.Should().Contain(i => i.FilePath == "staged_new.json");

        status.Staged.Should().HaveCount(1);
        status.Staged.Should().Contain(i => i.FilePath == "staged_modified.json");

        status.Removed.Should().HaveCount(1);
        status.Removed.Should().Contain(i => i.FilePath == "staged_deleted.json");

        status.Missing.Should().HaveCount(1);
        status.Missing.Should().Contain(i => i.FilePath == "notstaged_deleted.json");

        status.Modified.Should().HaveCount(1);
        status.Modified.Should().Contain(i => i.FilePath == "notstaged_modified.json");

        status.Untracked.Should().HaveCount(1);
        status.Untracked.First().FilePath.Should().Be("someuntracked.json");

        status.Ignored.Should().HaveCount(1);
        status.Ignored.First().FilePath.Should().Be("someignored.txt");

        status.Unchanged.Should().HaveCount(0);
    }

    private static string GetResourceString(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream($"HansPeterGit.Tests.{resourceName}");

        if (stream == null)
            throw new ArgumentException($"Resource '{resourceName}' not found in assembly '{assembly}'.", nameof(resourceName));
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
