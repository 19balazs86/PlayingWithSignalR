using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PlayingWithSignalR.Hubs;
using PlayingWithSignalR.Models;
using Xunit;

namespace UnitTest
{
  public class MessageHubUnitTest
  {
    private readonly Mock<IHubCallerClients<IMessageClient>> _mockClients;
    private readonly Mock<IMessageClient> _mockMessageClient;
    private readonly Mock<HubCallerContext> _mockHubContext;

    private const string _messageText = "Hello World!";
    private const string _callerName  = "Sender Name";
    private Guid _callerId            = Guid.NewGuid();

    private readonly MessageHub SUT;

    public MessageHubUnitTest()
    {
      // Create mock objects.
      _mockClients       = new Mock<IHubCallerClients<IMessageClient>>(MockBehavior.Strict);
      _mockMessageClient = new Mock<IMessageClient>(MockBehavior.Strict);
      _mockHubContext    = new Mock<HubCallerContext>(MockBehavior.Strict);

      // Default arrange
      _mockClients.Setup(clients => clients.All)
        .Returns(_mockMessageClient.Object);

      _mockHubContext.Setup(c => c.UserIdentifier)
       .Returns(_callerId.ToString());

      _mockHubContext.Setup(c => c.User.Identity.Name)
        .Returns(_callerName);

      _mockMessageClient.Setup(mc => mc.ReceiveMessage(It.IsAny<Message>()))
        .Returns(Task.CompletedTask)
        .Verifiable();

      // Create MessageHub
      SUT = new MessageHub()
      {
        Clients = _mockClients.Object,
        Context = _mockHubContext.Object
      };
    }

    [Fact]
    public async Task SendMessageToAll()
    {
      // Act
      await SUT.SendMessageToAll(_messageText);

      // Assert
      _mockMessageClient.Verify(messageClient =>
        messageClient.ReceiveMessage(It.Is<Message>(m => checkMessage(m, false))), Times.Once);
    }

    [Fact]
    public async Task SendPrivateMessage()
    {
      Guid toUserId = Guid.NewGuid();

      // Arrange
      _mockClients.Setup(clients => clients.User(It.Is<string>(userId => userId == toUserId.ToString())))
        .Returns(_mockMessageClient.Object);

      // Act
      await SUT.SendPrivateMessage(toUserId, _messageText);

      // Assert
      _mockMessageClient.Verify(messageClient =>
        messageClient.ReceiveMessage(It.Is<Message>(m => checkMessage(m, true))), Times.Once);
    }

    private bool checkMessage(Message m, bool isPrivate)
      => m.UserId == _callerId && m.UserName == _callerName && m.Text == _messageText && m.IsPrivate == isPrivate;
  }
}
