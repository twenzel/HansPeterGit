using System.Diagnostics;
using System.Text;

namespace HansPeterGit.Authentication;

/// <summary>
/// Basic authentication
/// </summary>
public class BasicAuthentication : IAuthentication
{
    private readonly string _authenticationParameter;

    /// <summary>
    /// Creates a new instance of the <see cref="BasicAuthentication"/> class.
    /// </summary>
    /// <param name="userName">The user name used for basic authentication.</param>
    /// <param name="password">The password used for basic authentication.</param>
    public BasicAuthentication(string userName, string password)
    {
        var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
        var encodedToken = Convert.ToBase64String(byteArray);

        _authenticationParameter = $"-c http.extraheader=\"AUTHORIZATION: Basic {encodedToken}\"";
    }

    /// <inheritdoc/>
    public void AddAuthentication(ProcessStartInfo startInfo)
    {
        startInfo.Arguments = $"{_authenticationParameter} {startInfo.Arguments}";
    }
}
