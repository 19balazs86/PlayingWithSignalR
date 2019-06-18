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
    private const string _messageToSend = "Hello World!";

    public HubIntegrationTest(WebApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SendMessageToAll()
    {
      // Arrange
      int counter = 0;
      Message receivedMessage = null;

      HubConnection connection1 = await getHubConnectionAsync(TestUsers.User1);
      HubConnection connection2 = await getHubConnectionAsync(TestUsers.User2);

      connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => receivedMessage = msg);
      connection2.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++); // +1

      // Act
      await connection1.InvokeAsync(nameof(IMessageHub.SendMessageToAll), _messageToSend);

      // Assert
      Assert.Equal(1, counter);
      Assert.NotNull(receivedMessage);
      Assert.Equal(_messageToSend, receivedMessage.Text);
      Assert.Equal(TestUsers.User1.Id, receivedMessage.UserId);
      Assert.Equal(TestUsers.User1.Name, receivedMessage.UserName);
      Assert.False(receivedMessage.IsPrivate);
    }

    [Fact]
    public async Task SendPrivateMessage()
    {
      // Arrange
      int counter = 0;
      Message receivedMessage = null;

      HubConnection connection1   = await getHubConnectionAsync(TestUsers.User1);
      HubConnection connection2_1 = await getHubConnectionAsync(TestUsers.User2);
      HubConnection connection2_2 = await getHubConnectionAsync(TestUsers.User2);

      connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++); // +0.
      connection2_1.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => receivedMessage = msg);
      connection2_2.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++); // +1

      // Act: User1 send a private message to User2, who has 2 connections.
      await connection1.InvokeAsync(nameof(IMessageHub.SendPrivateMessage), TestUsers.User2.Id, _messageToSend);

      await Task.Delay(100); // Waiting for the message to arrive.

      // Assert
      Assert.Equal(1, counter); // User1 did not get message.
      Assert.NotNull(receivedMessage);
      Assert.Equal(_messageToSend, receivedMessage.Text);
      Assert.Equal(TestUsers.User1.Id, receivedMessage.UserId);
      Assert.Equal(TestUsers.User1.Name, receivedMessage.UserName);
      Assert.True(receivedMessage.IsPrivate);
    }

    [Fact]
    public async Task NotificationController()
    {
      // Arrange
      _testUser = TestUsers.User1;

      Notification notification = new Notification { Message = _messageToSend };

      int counter = 0;
      Message receivedMessage = null;

      HubConnection connection1 = await getHubConnectionAsync(TestUsers.User1);
      HubConnection connection2 = await getHubConnectionAsync(TestUsers.User2);

      connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => counter++); // +1
      connection2.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => receivedMessage = msg);

      // Act: User1 initiates an HTTP call to send a notification for everyone.
      HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Notification", notification);

      // Assert
      Assert.True(response.IsSuccessStatusCode);
      Assert.Equal(1, counter);
      Assert.NotNull(receivedMessage);
      Assert.Equal(notification.Message, receivedMessage.Text);
      Assert.Equal(_testUser.Id, receivedMessage.UserId);
      Assert.Equal(_testUser.Name, receivedMessage.UserName);
      Assert.False(receivedMessage.IsPrivate);
    }

    private Task<HubConnection> getHubConnectionAsync(UserModel user)
      => getHubConnectionAsync(MessageHub.Path, user);
  }
}
