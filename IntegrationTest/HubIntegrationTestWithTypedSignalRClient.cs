using IntegrationTest.Core;
using Microsoft.AspNetCore.SignalR.Client;
using PlayingWithSignalR.Hubs;
using PlayingWithSignalR.Models;
using System.Net.Http.Json;
using TypedSignalR.Client;
using Xunit;

namespace IntegrationTest;

public sealed class Receiver : IMessageClient
{
    private readonly CountdownEvent _countdownEvent;

    public Message ReceivedMessage { get; private set; }

    public Receiver(CountdownEvent countdownEvent)
    {
        _countdownEvent = countdownEvent;
    }

    public Task ReceiveMessage(Message message)
    {
        ReceivedMessage = message;

        _countdownEvent.Signal();

        return Task.CompletedTask;
    }
}

public sealed class HubIntegrationTestWithTypedSignalRClient : IntegrationTestBase
{
    private const string _messageToSend = "Hello World!";

    public HubIntegrationTestWithTypedSignalRClient(WebApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_all_users_receive_message_When_user_sends_it()
    {
        // Arrange
        var countdownEvent = new CountdownEvent(2);

        await using HubConnection connection1 = await getHubConnectionAsync(DummyUsers.User1);
        await using HubConnection connection2 = await getHubConnectionAsync(DummyUsers.User2);

        IMessageHub hubProxy = connection1.CreateHubProxy<IMessageHub>(cancellationToken: default);

        var receiverUser1 = new Receiver(countdownEvent);
        var receiverUser2 = new Receiver(countdownEvent);

        using var subscription1 = connection1.Register<IMessageClient>(receiverUser1);
        using var subscription2 = connection2.Register<IMessageClient>(receiverUser2);

        // Act
        await hubProxy.SendMessageToAll(_messageToSend);

        bool isCountdownSet = countdownEvent.Wait(1_000);

        // Assert
        Assert.True(isCountdownSet); // Assert.Equal(0, countdownEvent.CurrentCount);
        Assert.NotNull(receiverUser1.ReceivedMessage);
        Assert.NotNull(receiverUser2.ReceivedMessage);
        Assert.Equal(_messageToSend,        receiverUser1.ReceivedMessage.Text);
        Assert.Equal(DummyUsers.User1.Id,   receiverUser1.ReceivedMessage.UserId);
        Assert.Equal(DummyUsers.User1.Name, receiverUser1.ReceivedMessage.UserName);
        Assert.False(receiverUser1.ReceivedMessage.IsPrivate);
    }

    [Fact]
    public async Task Should_addressee_receive_message_When_private_message_is_sent()
    {
        // Arrange
        var countdownEvent = new CountdownEvent(2);

        await using HubConnection connection1   = await getHubConnectionAsync(DummyUsers.User1);
        await using HubConnection connection2_1 = await getHubConnectionAsync(DummyUsers.User2);
        await using HubConnection connection2_2 = await getHubConnectionAsync(DummyUsers.User2);

        IMessageHub hubProxy = connection1.CreateHubProxy<IMessageHub>(cancellationToken: default);

        var receiverUser1   = new Receiver(countdownEvent);
        var receiverUser2_1 = new Receiver(countdownEvent);
        var receiverUser2_2 = new Receiver(countdownEvent);

        using var subscription1 = connection1.Register<IMessageClient>(receiverUser1);
        using var subscription2 = connection2_1.Register<IMessageClient>(receiverUser2_1);
        using var subscription3 = connection2_2.Register<IMessageClient>(receiverUser2_2);

        // Act: User1 send a private message to User2, who has 2 connections.
        await hubProxy.SendPrivateMessage(DummyUsers.User2.Id, _messageToSend);

        bool isCountdownSet = countdownEvent.Wait(1_000);

        // Assert
        Assert.True(isCountdownSet); // Assert.Equal(0, countdownEvent.CurrentCount);
        Assert.Null(receiverUser1.ReceivedMessage);
        Assert.NotNull(receiverUser2_1.ReceivedMessage);
        Assert.NotNull(receiverUser2_2.ReceivedMessage);
        Assert.Equal(_messageToSend,        receiverUser2_1.ReceivedMessage.Text);
        Assert.Equal(DummyUsers.User1.Id,   receiverUser2_1.ReceivedMessage.UserId);
        Assert.Equal(DummyUsers.User1.Name, receiverUser2_1.ReceivedMessage.UserName);
        Assert.True(receiverUser2_1.ReceivedMessage.IsPrivate);
    }

    [Fact]
    public async Task Should_notify_all_users_When_WebAPI_notification_is_called()
    {
        // Arrange
        _testUser = DummyUsers.User1;

        var notification = new Notification { Message = _messageToSend };

        var countdownEvent = new CountdownEvent(2);

        await using HubConnection connection1 = await getHubConnectionAsync(DummyUsers.User1);
        await using HubConnection connection2 = await getHubConnectionAsync(DummyUsers.User2);

        var receiverUser1 = new Receiver(countdownEvent);
        var receiverUser2 = new Receiver(countdownEvent);

        using var subscription1 = connection1.Register<IMessageClient>(receiverUser1);
        using var subscription2 = connection2.Register<IMessageClient>(receiverUser2);

        // Act: User1 initiates an HTTP call to send a notification for everyone.
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Notification", notification);

        bool isCountdownSet = countdownEvent.Wait(1_000);

        // Assert
        Assert.True(isCountdownSet);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(0, countdownEvent.CurrentCount);
        Assert.NotNull(receiverUser1.ReceivedMessage);
        Assert.NotNull(receiverUser2.ReceivedMessage);
        Assert.Equal(_messageToSend,        receiverUser1.ReceivedMessage.Text);
        Assert.Equal(DummyUsers.User1.Id,   receiverUser1.ReceivedMessage.UserId);
        Assert.Equal(DummyUsers.User1.Name, receiverUser1.ReceivedMessage.UserName);
        Assert.False(receiverUser1.ReceivedMessage.IsPrivate);
    }
}
