using System.Diagnostics;
using System.Text;

namespace HansPeterGit.Authentication;

/// <summary>
/// Basic authentication
/// </summary>
public class BasicAuthentication : IAuthentication
{
    private readonly string _authenticationParameter;

    public BasicAuthentication(string userName, string password)
    {
        var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
        var encodedToken = Convert.ToBase64String(byteArray);

        _authenticationParameter = $"-c http.extraheader=\"AUTHORIZATION: Basic {encodedToken}\"";
    }

    public void AddAuthentication(ProcessStartInfo startInfo)
    {
        startInfo.Arguments = $"{_authenticationParameter} {startInfo.Arguments}";
    }
}
