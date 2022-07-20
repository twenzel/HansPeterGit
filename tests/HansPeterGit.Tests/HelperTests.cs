using FluentAssertions;
using NUnit.Framework;

namespace HansPeterGit.Tests;
public class HelperTests
{
    public class FormatWithCredentials : HelperTests
    {
        [Test]
        public void Returns_Url_With_User_And_Password()
        {
            var url = Helper.FormatWithCredentials("http://gitub.com/test.git", "user", "password");
            url.Should().Be("http://user:password@gitub.com/test.git");
        }

        [Test]
        public void Returns_Url_With_Blank_Correctly()
        {
            var url = Helper.FormatWithCredentials("http://gitub.com/test%20repository.git", "user", "password");
            url.Should().Be("http://user:password@gitub.com/test%20repository.git");
        }

        [Test]
        public void Returns_Url_With_Password_Only()
        {
            var url = Helper.FormatWithCredentials("http://gitub.com/test.git", "password", "");
            url.Should().Be("http://password@gitub.com/test.git");
        }
    }
}
