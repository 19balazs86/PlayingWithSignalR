using PlayingWithSignalR.Models;

namespace PlayingWithSignalR.Hubs;

public interface IMessageClient
{
    Task ReceiveMessage(Message message);
}

public interface IMessageHub
{
    Task SendMessageToAll(string message);

    Task SendPrivateMessage(Guid userId, string message);
}