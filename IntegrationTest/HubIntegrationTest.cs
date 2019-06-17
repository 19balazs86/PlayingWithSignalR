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
      Assert.False(messageToReceive.IsPrivate);
    }

    [Fact]
    public async Task SendPrivateMessage()
    {
      // Arrange
      int counter = 0;
      const string messageToSend = "Hello World!";
      Message messageToReceive   = null;

      UserModel user1 = TestUsers.User1;
      UserModel user2 = TestUsers.User2;

      HubConnection connection1 = await getHubConnectionAsync(user1);
      HubConnection connection2 = await getHubConnectionAsync(user2);

      connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++);
      connection2.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => messageToReceive = msg);

      // Act
      await connection1.InvokeAsync(nameof(IMessageHub.SendPrivateMessage), user2.Id, messageToSend);

      // Assert
      Assert.Equal(0, counter);
      Assert.NotNull(messageToReceive);
      Assert.Equal(messageToSend, messageToReceive.Text);
      Assert.Equal(user1.Id, Guid.Parse(messageToReceive.UserId));
      Assert.Equal(user1.Name, messageToReceive.UserName);
      Assert.True(messageToReceive.IsPrivate);
    }

    private Task<HubConnection> getHubConnectionAsync(UserModel user)
      => getHubConnectionAsync(MessageHub.Path, user);
  }
}
