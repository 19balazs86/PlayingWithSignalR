using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace IntegrationTest
{
  public class IntegrationTestBase : IClassFixture<WebApiFactory>
  {
    private readonly WebApiFactory _factory;

    protected HttpClient _httpClient => _factory.HttpClient;

    protected TestServer _testServer => _factory.Server;

    public IntegrationTestBase(WebApiFactory factory)
    {
      _factory = factory;
    }

    protected virtual async Task<HubConnection> getHubConnectionAsync(string hubName)
    {
      HubConnection hubConnection = new HubConnectionBuilder()
        .WithUrl($"http://localhost/{hubName}", options =>
          options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler())
        .Build();

      await hubConnection.StartAsync();

      return hubConnection;
    }
  }
}
