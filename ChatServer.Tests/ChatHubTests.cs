using ChatServer.Hubs;
using ChatServer.Services;
using Xunit;
using Moq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace ChatServer.Tests
{
    public class ChatHubTests
    {
        public ChatHub Hub { get; }
        public FileMessageHistory Service { get; }

        public ChatHubTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var fullFilePath = $"{currentDirectory}/History.txt";
            Service = new FileMessageHistory(fullFilePath);
            var connectedUsersService = new ConnectedUsersService();

            Hub = new ChatHub(Service, connectedUsersService);
        }

        [Fact]
        public async Task ServerSendMessage_ConnectedClientsReceiveIt()
        {
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            Hub.Clients = mockClients.Object;

            await Hub.SendMessage("John", "Hi");

            mockClients.Verify(clients => clients.All, Times.Once());
            mockClientProxy.Verify(client =>
                client.SendCoreAsync("ReceiveMessage", It.Is<object[]>(param =>
                    param != null && param.Length == 2 && (string)param[0] == "John" && (string)param[1] == "Hi"),
                    default(CancellationToken)), Times.Once());
        }

        [Fact]
        public async Task ServerSendMessageHistory_CalledClientReceiveIt()
        {
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.Caller).Returns(mockClientProxy.Object);
            Hub.Clients = mockClients.Object;

            await Hub.GetMessageHistory();
            var messages = new List<string>(await Service.GetMessageHistoryAsync());

            mockClients.Verify(clients => clients.Caller, Times.Once());
            mockClientProxy.Verify(client =>
                client.SendCoreAsync("ReceiveMessageHistory", It.Is<object[]>(param =>
                    param != null && ((List<string>)param[0]).Count == messages.Count),
                    default(CancellationToken)), Times.Once());
        }
    }
}