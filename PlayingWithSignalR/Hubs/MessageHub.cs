using Microsoft.AspNetCore.SignalR;
using PlayingWithSignalR.Models;

namespace PlayingWithSignalR.Hubs;

public sealed class MessageHub : Hub<IMessageClient>, IMessageHub
{
    public const string Path = "/hub/messages";

    // https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs#the-context-object
    // https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?#use-claims-to-customize-identity-handling
    // SignalR uses the ClaimTypes.NameIdentifier from the ClaimsPrincipal
    private Guid _userIdentifier => Guid.Parse(Context.UserIdentifier);

    public override Task OnConnectedAsync()
    {
        string connId = Context.ConnectionId;

        //await Clients.Client(connId).ReceiveMessage(msg);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToAll(string message)
    {
        var msg = new Message
        {
            UserId   = _userIdentifier,
            UserName = Context.User.Identity.Name,
            Text     = message
        };

        //await Clients.All.SendAsync("ReceiveMessage", msg);
        await Clients.All.ReceiveMessage(msg);
    }

    public async Task SendPrivateMessage(Guid userId, string message)
    {
        var msg = new Message
        {
            UserId    = _userIdentifier,
            UserName  = Context.User.Identity.Name,
            Text      = message,
            IsPrivate = true
        };

        await Clients.User(userId.ToString()).ReceiveMessage(msg);
    }
}
