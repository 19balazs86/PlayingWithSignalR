using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PlayingWithSignalR.Hubs;
using PlayingWithSignalR.Models;
using Xunit;

namespace IntegrationTest
{
  public class HubIntegrationTest : IntegrationTestBase
  {
    public HubIntegrationTest(WebApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ReceiveMessage()
    {
      // Arrange
      const string messageToSend = "Hello World!";
      string messageToReceive    = null;

      HubConnection connection = await getHubConnectionAsync(TestUsers.User1);

      connection.On<string>(nameof(IMessageClient.ReceiveMessage), msg => messageToReceive = msg);

      // Act
      await connection.InvokeAsync(nameof(IMessageHub.SendMessageToAll), messageToSend);

      // Assert
      Assert.Equal(messageToSend, messageToReceive);
    }

    private Task<HubConnection> getHubConnectionAsync(UserModel user)
      => getHubConnectionAsync(MessageHub.Path, user);
  }
}
