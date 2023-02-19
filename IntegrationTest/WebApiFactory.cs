using Microsoft.AspNetCore.Mvc.Testing;
using PlayingWithSignalR;
using PlayingWithSignalR.Models;

namespace IntegrationTest;

public sealed class WebApiFactory : WebApplicationFactory<Program>
{
    public UserModel TestUser { get; set; }

    public HttpClient HttpClient { get; private set; }

    public WebApiFactory()
    {
        // Need to create a client for the Server property to be available.
        HttpClient = CreateDefaultClient(new AuthDelegatingHandler(() => TestUser?.ToClaims()));
    }
}
