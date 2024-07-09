using PlayingWithSignalR.Models;

namespace IntegrationTest.Core;

public static class DummyUsers
{
    public static UserModel User1 { get; } = new UserModel(Guid.NewGuid(), "User 1");
    public static UserModel User2 { get; } = new UserModel(Guid.NewGuid(), "User 2");
    public static UserModel User3 { get; } = new UserModel(Guid.NewGuid(), "User 3");
}
