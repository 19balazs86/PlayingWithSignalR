using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlayingWithSignalR.Hubs;
using Xunit;

namespace IntegrationTest
{
  public class HubIntegrationTest : IntegrationTestBase
  {
    private const string _hubName = "messages";

    public HubIntegrationTest(WebApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ReceiveMessage()
    {
      // Arrange
      const string messageToSend = "Hello World!";
      string messageToReceive    = null;

      HubConnection connection = await getHubConnectionAsync();

      connection.On<string>(nameof(IMessageClient.ReceiveMessage), msg => messageToReceive = msg);

      // Act
      await connection.InvokeAsync(nameof(IMessageHub.SendMessageToAll), messageToSend);

      // Assert
      Assert.Equal(messageToSend, messageToReceive);
    }

    private Task<HubConnection> getHubConnectionAsync()
      => getHubConnectionAsync(_hubName);
  }
}
