using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using PlayingWithSignalR.Infrastructure;
using PlayingWithSignalR.Models;
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

    protected virtual async Task<HubConnection> getHubConnectionAsync(string hubName, UserModel user)
    {
      HubConnection hubConnection = new HubConnectionBuilder()
        .WithUrl($"http://localhost{hubName}", options =>
        {
          options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
          options.AccessTokenProvider       = () => getToken(user);
        })
        .Build();

      await hubConnection.StartAsync();

      return hubConnection;
    }

    private static Task<string> getToken(UserModel user)
      => Task.FromResult(TokenFactory.CreateToken(user.ToClaims()));
  }
}
