using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PlayingWithSignalR.Models;
using Xunit;

namespace UnitTest
{
  public class NonStronglyTypedHub : Hub
  {
    public async Task SendMessageToAll(string message)
    {
      Message msg = new Message
      {
        UserId   = Guid.NewGuid(),
        UserName = "UserName",
        Text     = message
      };

      await Clients.All.SendAsync("ReceiveMessage", msg);
    }
  }

  public class NonStronglyTypedHubTest
  {
    private const string _messageText = "Hello World!";

    private readonly Mock<IHubCallerClients> _mockClients;
    private readonly Mock<IClientProxy> _mockClientProxy;

    private readonly NonStronglyTypedHub SUT;

    public NonStronglyTypedHubTest()
    {
      _mockClients     = new Mock<IHubCallerClients>();
      _mockClientProxy = new Mock<IClientProxy>();

      _mockClients.Setup(clients => clients.All)
        .Returns(_mockClientProxy.Object);

      SUT = new NonStronglyTypedHub { Clients = _mockClients.Object };
    }

    [Fact]
    public async Task SendMessageToAll()
    {
      // Act
      await SUT.SendMessageToAll(_messageText);

      // Assert
      _mockClients.Verify(clients => clients.All, Times.Once);

      _mockClientProxy.Verify(clientProxy =>
        clientProxy.SendCoreAsync(
          "ReceiveMessage",
          It.Is<object[]>(o => o != null && o.Length == 1 && checkMessage(o[0] as Message)),
          It.IsAny<CancellationToken>()),
        Times.Once);
    }

    private bool checkMessage(Message m)
      => m != null && m.Text == _messageText && m.IsPrivate == false;
  }
}
