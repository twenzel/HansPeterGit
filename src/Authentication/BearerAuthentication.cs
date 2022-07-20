using System.Diagnostics;

namespace HansPeterGit.Authentication;

/// <summary>
/// Bearer authentication
/// </summary>
public class BearerAuthentication : IAuthentication
{
    private readonly string _authenticationParameter;

    public BearerAuthentication(string token)
    {
        _authenticationParameter = $"-c http.extraheader=\"AUTHORIZATION: bearer {token}\"";
    }

    public void AddAuthentication(ProcessStartInfo startInfo)
    {
        startInfo.Arguments = $"{_authenticationParameter} {startInfo.Arguments}";
    }
}
