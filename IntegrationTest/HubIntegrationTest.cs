using IntegrationTest.Core;
using Microsoft.AspNetCore.SignalR.Client;
using PlayingWithSignalR.Hubs;
using PlayingWithSignalR.Models;
using System.Net.Http.Json;
using Xunit;

namespace IntegrationTest;

public sealed class HubIntegrationTest : IntegrationTestBase
{
    private const string _messageToSend = "Hello World!";

    public HubIntegrationTest(WebApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_all_users_receive_message_When_user_sends_it()
    {
        // Arrange
        var countdownEvent      = new CountdownEvent(2);
        Message receivedMessage = null;

        await using HubConnection connection1 = await getHubConnectionAsync(DummyUsers.User1);
        await using HubConnection connection2 = await getHubConnectionAsync(DummyUsers.User2);

        connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), msg =>
        {
            receivedMessage = msg;
            countdownEvent.Signal();
        });

        connection2.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => countdownEvent.Signal());

        // Act
        await connection1.InvokeAsync(nameof(IMessageHub.SendMessageToAll), _messageToSend);

        bool isCountdownSet = countdownEvent.Wait(1_000);

        // Assert
        Assert.True(isCountdownSet); // Assert.Equal(0, countdownEvent.CurrentCount);
        Assert.NotNull(receivedMessage);
        Assert.Equal(_messageToSend,        receivedMessage.Text);
        Assert.Equal(DummyUsers.User1.Id,   receivedMessage.UserId);
        Assert.Equal(DummyUsers.User1.Name, receivedMessage.UserName);
        Assert.False(receivedMessage.IsPrivate);
    }

    [Fact]
    public async Task Should_addressee_receive_message_When_private_message_is_sent()
    {
        // Arrange
        int counter = 0;
        Message receivedMessage = null;

        await using HubConnection connection1   = await getHubConnectionAsync(DummyUsers.User1);
        await using HubConnection connection2_1 = await getHubConnectionAsync(DummyUsers.User2);
        await using HubConnection connection2_2 = await getHubConnectionAsync(DummyUsers.User2);

        connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), _     => Interlocked.Increment(ref counter)); // +0.
        connection2_1.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => receivedMessage = msg);
        connection2_2.On<Message>(nameof(IMessageClient.ReceiveMessage), _   => Interlocked.Increment(ref counter)); // +1

        // Act: User1 send a private message to User2, who has 2 connections.
        await connection1.InvokeAsync(nameof(IMessageHub.SendPrivateMessage), DummyUsers.User2.Id, _messageToSend);

        await Task.Delay(100); // Waiting for the message to arrive.

        // Assert
        Assert.Equal(1, counter); // User1 did not get message.
        Assert.NotNull(receivedMessage);
        Assert.Equal(_messageToSend,        receivedMessage.Text);
        Assert.Equal(DummyUsers.User1.Id,   receivedMessage.UserId);
        Assert.Equal(DummyUsers.User1.Name, receivedMessage.UserName);
        Assert.True(receivedMessage.IsPrivate);
    }

    [Fact]
    public async Task Should_notify_all_users_When_WebAPI_notification_is_called()
    {
        // Arrange
        _testUser = DummyUsers.User1;

        var notification = new Notification { Message = _messageToSend };

        var countdownEvent      = new CountdownEvent(2);
        Message receivedMessage = null;

        await using HubConnection connection1 = await getHubConnectionAsync(DummyUsers.User1);
        await using HubConnection connection2 = await getHubConnectionAsync(DummyUsers.User2);

        connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), _ => countdownEvent.Signal());
        connection2.On<Message>(nameof(IMessageClient.ReceiveMessage), msg =>
        {
            receivedMessage = msg;
            countdownEvent.Signal();
        });

        // Act: User1 initiates an HTTP call to send a notification for everyone.
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Notification", notification);

        bool isCountdownSet = countdownEvent.Wait(1_000);

        // Assert
        Assert.True(isCountdownSet);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(0, countdownEvent.CurrentCount);
        Assert.NotNull(receivedMessage);
        Assert.Equal(notification.Message, receivedMessage.Text);
        Assert.Equal(_testUser.Id,         receivedMessage.UserId);
        Assert.Equal(_testUser.Name,       receivedMessage.UserName);
        Assert.False(receivedMessage.IsPrivate);
    }
}
