using System;
using System.Threading.Tasks;
using ChatServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageHistoryService _messageHistoryService;
        private readonly IConnectedUsersService _connectedUsersService;

        public ChatHub(IMessageHistoryService messageHistoryService, IConnectedUsersService connectedUsersService)
        {
            _messageHistoryService = messageHistoryService;
            _connectedUsersService = connectedUsersService;
        }

        override public async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            string username = httpContext.Request.Query["username"];

            if (!_connectedUsersService.AddUser(Context.ConnectionId, username))
            {
                await Clients.Others.SendAsync("OnUserConnected", "Anonymous");
            }
            else
            {
                await Clients.Others.SendAsync("OnUserConnected", username);
            }
            await base.OnConnectedAsync();
        }

        override public async Task OnDisconnectedAsync(Exception exception)
        {
            var deletedUser = _connectedUsersService.RemoveUser(Context.ConnectionId);
            if (deletedUser != null)
                await Clients.Others.SendAsync("OnUserDisconnected", deletedUser.UserName);
            else await Clients.Others.SendAsync("OnUserDisconnected", "Anonymous");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string sender, string message)
        {
            await _messageHistoryService.AddMessageToHistoryAsync(message);
            await Clients.All.SendAsync("ReceiveMessage", sender, message);
        }

        public async Task GetMessageHistory()
        {
            var messagesHistory = await _messageHistoryService.GetMessageHistoryAsync();
            await Clients.Caller.SendAsync("ReceiveMessageHistory", messagesHistory);
        }
    }
}