using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using PlayingWithSignalR.Infrastructure;
using PlayingWithSignalR.Models;
using Xunit;

namespace IntegrationTest;

public class IntegrationTestBase : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _webApiFactory;

    protected HttpClient _httpClient => _webApiFactory.HttpClient;

    protected TestServer _testServer => _webApiFactory.Server;

    protected UserModel _testUser { get => _webApiFactory.TestUser; set => _webApiFactory.TestUser = value; }

    public IntegrationTestBase(WebApiFactory factory)
    {
        _webApiFactory = factory;

        _testUser = null;
    }

    protected virtual async Task<HubConnection> getHubConnectionAsync(string hubName, UserModel user)
    {
        HubConnection hubConnection = new HubConnectionBuilder()
            .WithUrl($"http://localhost{hubName}", options =>
            {
                options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                options.AccessTokenProvider = () => getToken(user);
            })
            .Build();

        await hubConnection.StartAsync();

        return hubConnection;
    }

    private static Task<string> getToken(UserModel user)
        => Task.FromResult(TokenFactory.CreateToken(user.ToClaims()));
}
