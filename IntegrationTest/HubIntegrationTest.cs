using System;
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
      Message messageToReceive    = null;

      UserModel user = TestUsers.User1;

      HubConnection connection = await getHubConnectionAsync(user);

      connection.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => messageToReceive = msg);

      // Act
      await connection.InvokeAsync(nameof(IMessageHub.SendMessageToAll), messageToSend);

      // Assert
      Assert.NotNull(messageToReceive);
      Assert.Equal(messageToSend, messageToReceive.Text);
      Assert.Equal(user.Id, Guid.Parse(messageToReceive.UserId));
      Assert.Equal(user.Name, messageToReceive.UserName);
    }

    private Task<HubConnection> getHubConnectionAsync(UserModel user)
      => getHubConnectionAsync(MessageHub.Path, user);
  }
}
