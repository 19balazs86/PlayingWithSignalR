using Microsoft.AspNetCore.Authentication.JwtBearer;
using PlayingWithSignalR.Infrastructure;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace IntegrationTest.Core;

public sealed class AuthDelegatingHandler : DelegatingHandler
{
    private readonly Func<IEnumerable<Claim>> _testUserClaimsFunc;

    public AuthDelegatingHandler(Func<IEnumerable<Claim>> testUserClaimsFunc)
        => _testUserClaimsFunc = testUserClaimsFunc;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancelToken)
    {
        IEnumerable<Claim> claims = _testUserClaimsFunc?.Invoke();

        if (claims is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme, TokenFactory.CreateToken(claims));

        return base.SendAsync(request, cancelToken);
    }
}
