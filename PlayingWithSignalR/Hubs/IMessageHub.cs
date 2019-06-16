using System.Threading.Tasks;

namespace PlayingWithSignalR.Hubs
{
  public interface IMessageHub
  {
    Task SendMessageToAll(string message);
  }
}
