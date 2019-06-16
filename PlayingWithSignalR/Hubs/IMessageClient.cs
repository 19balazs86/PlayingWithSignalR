using System.Threading.Tasks;

namespace PlayingWithSignalR.Hubs
{
  public interface IMessageClient
  {
    Task ReceiveMessage(string message);
  }
}
