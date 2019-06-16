using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using PlayingWithSignalR;

namespace IntegrationTest
{
  public class WebApiFactory : WebApplicationFactory<Startup>
  {
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
        .UseStartup<Startup>();
    }
  }
}
