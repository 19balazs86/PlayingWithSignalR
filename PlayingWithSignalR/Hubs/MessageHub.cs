using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PlayingWithSignalR.Models;

namespace PlayingWithSignalR.Hubs
{
  [Authorize]
  public class MessageHub : Hub<IMessageClient>, IMessageHub
  {
    public const string Path = "/hub/messages";

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
      Message msg = new Message
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
      Message msg = new Message
      {
        UserId    = _userIdentifier,
        UserName  = Context.User.Identity.Name,
        Text      = message,
        IsPrivate = true
      };

      await Clients.User(userId.ToString()).ReceiveMessage(msg);
    }
  }
}
