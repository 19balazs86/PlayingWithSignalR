using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PlayingWithSignalR.Hubs
{
  public class MessageHub : Hub<IMessageClient>, IMessageHub
  {
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
