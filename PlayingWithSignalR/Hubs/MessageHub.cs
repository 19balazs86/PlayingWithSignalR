using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PlayingWithSignalR.Hubs
{
  [Authorize]
  public class MessageHub : Hub<IMessageClient>, IMessageHub
  {
    public const string Path = "/hub/messages";

    public override Task OnConnectedAsync()
    {
      var id = Context.ConnectionId;

      return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
      return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToAll(string message)
    {
      //await Clients.All.SendAsync("ReceiveMessage", message);
      await Clients.All.ReceiveMessage(message);
    }
  }
}
