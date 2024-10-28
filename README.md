# Playing with SignalR

This is a .NET Web API example for building a back-end [SignalR](https://docs.microsoft.com/en-ie/aspnet/core/signalr/introduction) service

## Resources
- [SignalR samples](https://github.com/aspnet/AzureSignalR-samples) 👤*aspnet*
- [Practical ASP.NET Core SignalR](https://codeopinion.com/practical-asp-net-core-signalr) 📓*CodeOpinion*
- [SignalR: To Chat and Beyond](https://www.youtube.com/watch?v=i3RXbOY6-0I) 📽️*52m - David Pine presentation*
- [TypedSignalR.Client](https://github.com/nenoNaninu/TypedSignalR.Client) 👤*nenoNaninu*
- [Creating sticky notes Blazor WASM application](https://kristoffer-strube.dk/post/typed-signalr-clients-making-type-safe-real-time-communication-in-dotnet) 📓*Kristoffer Strube - Typed SignalR clients using SourceGenerator*

## Code examples

### MessageHub
- The web application has a simple and strongly-typed Hub definition named `IMessageClient`, which implements the `IMessageHub` interface.
- Both interfaces make it easier to implement the hub and write unit and integration tests.
- To connect to the Hub, authentication is required, and I have used JWT for this purpose.

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

### UnitTest (strongly typed Hub)
- Mocking the strongly typed Hub becomes easier, allowing you to validate the object you intend to send to clients.

```csharp
_mockMessageClient.Verify(mc => mc.ReceiveMessage(It.Is<Message>(m => ..., Times.Once);
```

### UnitTest (non strongly typed Hub)
- It is still possible to verify the message you send, but it is not as elegant as the strongly-typed version.

```csharp
_mockClients.Verify(clients => clients.All, Times.Once);

_mockClientProxy.Verify(clientProxy =>
    clientProxy.SendCoreAsync(
        "ReceiveMessage",
        It.Is<object[]>(o => o != null && o.Length == 1 && checkMessage(o[0] as Message)),
        It.IsAny<CancellationToken>()),
    Times.Once);
```

### IntegrationTest
- Using the built-in `TestServer` like in my [PlayingWithTestHost](https://github.com/19balazs86/PlayingWithTestHost) repository.
- Bypassing ASP.NET Core authorization in integration tests.
- Leveraging the interfaces with the `nameof` operator can also be advantageous for us.

```csharp
// Subscribe for a message.
connection1.On<Message>(nameof(IMessageClient.ReceiveMessage), msg => ...);

// Send a message.
connection1.InvokeAsync(nameof(IMessageHub.SendMessageToAll), new Message(...));
```