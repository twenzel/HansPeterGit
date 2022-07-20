using System.Diagnostics;

namespace HansPeterGit.Authentication;

/// <summary>
/// Bearer authentication
/// </summary>
public class BearerAuthentication : IAuthentication
{
    private readonly string _authenticationParameter;

    /// <summary>
    /// Creates a new instance of <see cref="BearerAuthentication"/>
    /// </summary>
    /// <param name="token">The bearer token</param>
    public BearerAuthentication(string token)
    {
        _authenticationParameter = $"-c http.extraheader=\"AUTHORIZATION: bearer {token}\"";
    }

    /// <inheritdoc/>
    public void AddAuthentication(ProcessStartInfo startInfo)
    {
        startInfo.Arguments = $"{_authenticationParameter} {startInfo.Arguments}";
    }
}
