using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IntegrationTest
{
  public class FakeUserFilter : IAsyncActionFilter
  {
    private readonly Func<IEnumerable<Claim>> _testUserClaimsFunc;

    public FakeUserFilter(Func<IEnumerable<Claim>> testUserClaimsFunc)
      => _testUserClaimsFunc = testUserClaimsFunc;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      IEnumerable<Claim> claims = _testUserClaimsFunc?.Invoke() ?? Enumerable.Empty<Claim>();

      var claimsIdentity = new ClaimsIdentity(claims);

      context.HttpContext.User = new ClaimsPrincipal(claimsIdentity);

      await next();
    }
  }
}
