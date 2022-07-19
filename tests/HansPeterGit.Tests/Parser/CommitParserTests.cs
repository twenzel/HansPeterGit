using FluentAssertions;
using HansPeterGit.Parser;
using NUnit.Framework;

namespace HansPeterGit.Tests.Git.Parser;

public class CommitParserTests
{
    [Test]
    public void Can_Parse_Single_Line()
    {
        var commit = CommitParser.Parse("[master da23f5c]  Update api documentation");
        commit.Branch.Should().Be("master");
        commit.Id.Should().Be("da23f5c");
        commit.Message.Should().Be("Update api documentation");
    }

    [Test]
    public void Can_Parse_Multi_Line()
    {
        var commit = CommitParser.Parse("[master da23f5c]  Update api documentation\nAuthor: API Documentation Publisher <developers@chg-meridian.com>\n1 file changed, 909 insertions(+)\ncreate mode 100644 reference / holograph.json");
        commit.Branch.Should().Be("master");
        commit.Id.Should().Be("da23f5c");
        commit.Message.Should().Be("Update api documentation");
    }
}
