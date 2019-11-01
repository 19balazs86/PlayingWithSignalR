using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlayingWithSignalR.Hubs;

namespace PlayingWithSignalR
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      services.AddJwtAuthentication();

      services.AddSignalR(options => options.EnableDetailedErrors = true);

      services.AddAuthorization(options =>
      {
        // https://docs.microsoft.com/en-ie/aspnet/core/migration/22-to-30?view=aspnetcore-3.0&tabs=visual-studio#authorization
        // FallbackPolicy is initially configured to allow requests without authorization.
        // Override it to always require authentication on all endpoints except when [AllowAnonymous].
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();
      });
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();

      app.UseRouting();

      //app.UseSignalRClientMiddleware();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHub<MessageHub>(MessageHub.Path);
      });
    }
  }
}
