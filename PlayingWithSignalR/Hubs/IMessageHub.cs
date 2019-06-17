using System;
using System.Threading.Tasks;

namespace PlayingWithSignalR.Hubs
{
  public interface IMessageHub
  {
    Task SendMessageToAll(string message);

    Task SendPrivateMessage(Guid userId, string message);
  }
}
