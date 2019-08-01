using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace PlayingWithSignalR
{
  public static class ExceptionMiddlewareExtensions
  {
    public static IApplicationBuilder UseSignalRClientMiddleware(this IApplicationBuilder app)
      => app.UseMiddleware<SignalRClientMiddleware>();
  }

  public class SignalRClientMiddleware
  {
    private readonly RequestDelegate _next;

    public SignalRClientMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext httpContext)
    {
      HttpRequest request = httpContext.Request;

      // Web sockets cannot pass headers. We can take the access token from query param and
      // add it to the header before the authentication middleware runs.
      // Or use the solution in the AuthenticationExtension.
      if (request.Path.StartsWithSegments("/hub", StringComparison.OrdinalIgnoreCase) &&
          request.Query.TryGetValue("access_token", out var accessToken))
      {
        request.Headers.Add(HeaderNames.Authorization, $"{JwtBearerDefaults.AuthenticationScheme} {accessToken}");
      }

      await _next(httpContext);
    }
  }
}
