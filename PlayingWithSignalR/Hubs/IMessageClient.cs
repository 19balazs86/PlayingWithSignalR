using System.Threading.Tasks;
using PlayingWithSignalR.Models;

namespace PlayingWithSignalR.Hubs
{
  public interface IMessageClient
  {
    Task ReceiveMessage(Message message);
  }
}
