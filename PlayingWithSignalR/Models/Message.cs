namespace PlayingWithSignalR.Models;

public sealed class Message
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Text { get; set; }
    public bool IsPrivate { get; set; } = false;
}
