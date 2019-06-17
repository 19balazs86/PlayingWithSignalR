using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PlayingWithSignalR.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class AuthenticationExtension
  {
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
      services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
          options.TokenValidationParameters = TokenFactory.TokenValidationParameters;
          // Use this to obtain the token from web sockets clients or use the SignalRClientMiddleware.
          options.Events = new JwtBearerEvents
          {
            OnMessageReceived = context =>
            {
              if (context.Request.Query.TryGetValue("access_token", out var accessToken))
                  context.Token = accessToken.ToString();

              return Task.CompletedTask;
            }
          };
        });

      return services;
    }
  }
}
