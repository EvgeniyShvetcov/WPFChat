using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatClient.Helpers;
using System.Threading;

namespace ChatClient.Services
{
    public class ChatConnectionService
    {
        public string ConnectionPath { get; set; }
        public bool IsConnected
        {
            get
            {
                return _connection?.State == HubConnectionState.Connected;
            }
        }

        public delegate void UserMovingHandler(string username);
        public delegate void MessageReceivingHandler(string sender, string message);
        public delegate void MessagesHistoryGetting(IEnumerable<string> messages);
        public delegate void ConnectionClosingHandler();

        public event UserMovingHandler OnUserConnected;
        public event UserMovingHandler OnUserDisconnected;
        public event MessageReceivingHandler OnMessageReceived;
        public event MessagesHistoryGetting OnMessagesHistoryReceived;
        public event ConnectionClosingHandler OnConnectionClosed;

        public ChatConnectionService(string connectionPath, IDictionary<string, string> queryParameters = null)
        {
            if (!string.IsNullOrEmpty(connectionPath))
            {
                string queryParams = QueryParametersHelper.ToQueryString(queryParameters);
                connectionPath += queryParams;
                _connection = new HubConnectionBuilder()
                            .WithUrl(connectionPath)
                            .Build();

                EventsInitialization();

                ConnectionPath = connectionPath;
            }
        }

        public Task StartConnection(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _connection?.StartAsync(cancellationToken);
        }

        public Task StopConnection(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!IsConnected) return Task.CompletedTask;
            return _connection?.StopAsync(cancellationToken);
        }

        public async Task SendMessage(string sender, string message)
        {
            if (!IsConnected) await Task.CompletedTask;
            await _connection?.SendAsync("SendMessage", sender, message);
        }

        public async Task GetMessageHistory()
        {
            if (!IsConnected) await Task.CompletedTask;
            await _connection?.InvokeAsync("GetMessageHistory");
        }

        private void EventsInitialization()
        {
            _connection.Closed += Connection_Closed;

            _connection.On<string>("OnUserConnected", (username) =>
            {
                OnUserConnected(username);
            });
            _connection.On<string>("OnUserDisconnected", (username) =>
            {
                OnUserDisconnected(username);
            });
            _connection.On<string, string>("ReceiveMessage", (sender, message) =>
            {
                OnMessageReceived(sender, message);
            });
            _connection.On<IEnumerable<string>>("ReceiveMessageHistory", (messages) =>
            {
                OnMessagesHistoryReceived(messages);
            });
        }

        private Task Connection_Closed(System.Exception arg)
        {
            OnConnectionClosed();
            return Task.CompletedTask;
        }

        private readonly HubConnection _connection;
    }
}
