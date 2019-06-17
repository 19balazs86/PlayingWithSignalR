using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PlayingWithSignalR;
using PlayingWithSignalR.Models;

namespace IntegrationTest
{
  public class WebApiFactory : WebApplicationFactory<Startup>
  {
    public UserModel TestUser { get; set; }

    public HttpClient HttpClient { get; private set; }

    public WebApiFactory()
    {
      // Need to create a client for the Server property to be available.
      HttpClient = CreateClient();
    }

    protected override IWebHostBuilder CreateWebHostBuilder()
    {
      return WebHost
        .CreateDefaultBuilder()
        .ConfigureTestServices(services =>
        {
          // This hack does not have any effect on the Hub. Just for the HTTP call.
          services.AddMvc(options =>
          {
            options.Filters.Add(new AllowAnonymousFilter());
            options.Filters.Add(new FakeUserFilter(() => TestUser?.ToClaims()));
          });
        })
        .UseStartup<Startup>();
    }
  }
}
