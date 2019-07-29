using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PlayingWithSignalR.Infrastructure;

namespace IntegrationTest
{
  public class AuthDelegatingHandler : DelegatingHandler
  {
    private readonly Func<IEnumerable<Claim>> _testUserClaimsFunc;

    public AuthDelegatingHandler(Func<IEnumerable<Claim>> testUserClaimsFunc)
      => _testUserClaimsFunc = testUserClaimsFunc;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancelToken)
    {
      IEnumerable<Claim> claims = _testUserClaimsFunc?.Invoke();

      if (claims != null)
        request.Headers.Authorization = new AuthenticationHeaderValue(
          JwtBearerDefaults.AuthenticationScheme, TokenFactory.CreateToken(claims));

      return base.SendAsync(request, cancelToken);
    }
  }
}
