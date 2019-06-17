using System.Net.Http;
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
      Assert.Equal(user.Id, messageToReceive.UserId);
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
      HubConnection connection2_1 = await getHubConnectionAsync(user2);
      HubConnection connection2_2 = await getHubConnectionAsync(user2);

      connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++);
      connection2_1.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => messageToReceive = msg);
      connection2_2.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++);

      // Act: User1 send a private message to User2, who has 2 connections.
      await connection1.InvokeAsync(nameof(IMessageHub.SendPrivateMessage), user2.Id, messageToSend);

      await Task.Delay(100); // Waiting for the message to arrive.

      // Assert
      Assert.Equal(1, counter); // User1 did not get message.
      Assert.NotNull(messageToReceive);
      Assert.Equal(messageToSend, messageToReceive.Text);
      Assert.Equal(user1.Id, messageToReceive.UserId);
      Assert.Equal(user1.Name, messageToReceive.UserName);
      Assert.True(messageToReceive.IsPrivate);
    }

    [Fact]
    public async Task NotificationController()
    {
      // Arrange
      _testUser = TestUsers.User1;

      Notification notification = new Notification { Message = "SendNotification" };

      int counter = 0;
      Message messageToReceive = null;

      HubConnection connection1 = await getHubConnectionAsync(TestUsers.User1);
      HubConnection connection2 = await getHubConnectionAsync(TestUsers.User2);

      connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++);
      connection2.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => messageToReceive = msg);

      // Act: User1 initiates an HTTP call to send a notification for everyone.
      HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Notification", notification);

      // Assert
      Assert.True(response.IsSuccessStatusCode);
      Assert.Equal(1, counter);
      Assert.NotNull(messageToReceive);
      Assert.Equal(notification.Message, messageToReceive.Text);
      Assert.Equal(_testUser.Id, messageToReceive.UserId);
      Assert.Equal(_testUser.Name, messageToReceive.UserName);
      Assert.False(messageToReceive.IsPrivate);
    }

    private Task<HubConnection> getHubConnectionAsync(UserModel user)
      => getHubConnectionAsync(MessageHub.Path, user);
  }
}
