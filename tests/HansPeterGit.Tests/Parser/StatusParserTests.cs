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

        status.Modified.Should().HaveCount(2);
        status.Modified.Should().Contain(i => i.FilePath == "package.json");
        status.Unchanged.Should().HaveCount(1);
        status.Unchanged.First().FilePath.Should().Be("sub/test.json");
        status.Untracked.Should().HaveCount(1);
        status.Untracked.First().FilePath.Should().Be("someuntracked.json");
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
