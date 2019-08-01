# Playing with SignalR

This .Net Core Web API is an example to build [SignalR](https://docs.microsoft.com/en-ie/aspnet/core/signalr/introduction?view=aspnetcore-3.0) service from a back-end perspective.

#### Resources
- Some basic knowledge from CodeOpinion: [Practical ASP.NET Core SignalR](https://codeopinion.com/practical-asp-net-core-signalr/).
- David Pine presentation (52min): [SignalR: To Chat and Beyond](https://www.youtube.com/watch?v=i3RXbOY6-0I).

#### MessageHub
- The web application has a very simple strongly typed Hub definition as `IMessageClient` and implements the `IMessageHub` interface.
- Both interfaces make your things easier in order to implement the hub and write unit and integration tests.
- Connect to the Hub needs to be authenticated, I used JWT for this purpose.

```csharp
[Authorize]
public class MessageHub : Hub<IMessageClient>, IMessageHub
```

```csharp
public interface IMessageClient
{
    Task ReceiveMessage(Message message);
}
```

```csharp
public interface IMessageHub
{
    Task SendMessageToAll(string message);

    Task SendPrivateMessage(Guid userId, string message);
}
```

#### UnitTest (strongly typed Hub)
- You can mock the strongly typed Hub easier and validate the object which you want to send to the clients.

```csharp
_mockMessageClient.Verify(mc => mc.ReceiveMessage(It.Is<Message>(m => ..., Times.Once);
```

#### UnitTest (non strongly typed Hub)
- It is still doable to verify the message which you send, just it is not as elegant as the strongly-typed version.

```csharp
_mockClients.Verify(clients => clients.All, Times.Once);

_mockClientProxy.Verify(clientProxy =>
    clientProxy.SendCoreAsync(
        "ReceiveMessage",
        It.Is<object[]>(o => o != null && o.Length == 1 && checkMessage(o[0] as Message)),
        It.IsAny<CancellationToken>()),
    Times.Once);
```

#### IntegrationTest
- Using the built-in `TestServer` like in my [PlayingWithTestHost](https://github.com/19balazs86/PlayingWithTestHost) repository.
- Bypassing ASP.NET Core authorization in integration tests.
- We also can gain an advantage by leveraging the interfaces with the `nameof` operator.

```csharp
// Subscribe for a message.
connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => ...);

// Send a message.
connection1.InvokeAsync(nameof(IMessageHub.SendMessageToAll), new Message(...));
```