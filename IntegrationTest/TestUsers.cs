using System;
using PlayingWithSignalR.Models;

namespace IntegrationTest
{
  public static class TestUsers
  {
    public static readonly UserModel User1;
    public static readonly UserModel User2;
    public static readonly UserModel User3;

    static TestUsers()
    {
      User1 = new UserModel(Guid.NewGuid(), "User 1");
      User2 = new UserModel(Guid.NewGuid(), "User 2");
      User3 = new UserModel(Guid.NewGuid(), "User 3");
    }
  }
}
