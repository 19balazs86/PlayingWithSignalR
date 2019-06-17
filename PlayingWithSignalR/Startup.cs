using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.AddJwtAuthentication();

      services.AddSignalR(options => options.EnableDetailedErrors = true);
    }
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();

      //app.UseSignalRClientMiddleware();

      app.UseAuthentication();

      app.UseSignalR(builder => builder.MapHub<MessageHub>(MessageHub.Path));

      app.UseMvc();
    }
  }
}
