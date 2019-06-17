using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlayingWithSignalR.Hubs;
using PlayingWithSignalR.Models;

namespace PlayingWithSignalR.Controllers
{
  [Route("[controller]")]
  [ApiController]
  [Authorize]
  public class NotificationController : ControllerBase
  {
    private readonly IHubContext<MessageHub, IMessageClient> _messageHub;

    // Send messages from outside a hub
    // https://docs.microsoft.com/en-ie/aspnet/core/signalr/hubcontext?view=aspnetcore-2.2
    public NotificationController(IHubContext<MessageHub, IMessageClient> messageHub)
    {
      _messageHub = messageHub;
    }

    [HttpPost]
    public async Task Post(Notification notification)
    {
      UserModel user = new UserModel(HttpContext.User.Claims);

      Message msg = new Message
      {
        UserId   = user.Id,
        UserName = user.Name,
        Text     = notification.Message
      };

      await _messageHub.Clients.All.ReceiveMessage(msg);
    }
  }
}
