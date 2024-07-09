using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using PlayingWithSignalR.Hubs;
using PlayingWithSignalR.Infrastructure;
using PlayingWithSignalR.Models;
using Xunit;

namespace IntegrationTest.Core;

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

    protected virtual async Task<HubConnection> getHubConnectionAsync(UserModel user)
    {
        const string url = $"http://localhost{MessageHub.Path}";

        HubConnection hubConnection = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                options.AccessTokenProvider = () => getToken(user);
            })
            .Build();

        await hubConnection.StartAsync();

        // hubConnection.ConnectionId

        return hubConnection;
    }

    private static Task<string> getToken(UserModel user)
        => Task.FromResult(TokenFactory.CreateToken(user.ToClaims()));
}
