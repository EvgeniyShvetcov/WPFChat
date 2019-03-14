using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ChatClient.Models;
using ChatClient.Services;
using ChatClient.Helpers;
using ChatClient.Views;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User User { get; set; }
        public ChatConnectionService ChatService { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }
        ~MainWindow()
        {
            if (ChatService != null)
            {
                ChatService.OnUserConnected -= ChatService_OnUserConnected;
                ChatService.OnUserDisconnected -= ChatService_OnUserDisconnected;
                ChatService.OnMessageReceived -= ChatService_OnMessageReceived;
                ChatService.OnMessagesHistoryReceived -= ChatService_OnMessagesHistoryReceived;
                ChatService.OnConnectionClosed -= ChatService_OnConnectionClosed;
            }
        }

        private void InitializeChatEvents()
        {
            if (ChatService != null)
            {
                ChatService.OnUserConnected += ChatService_OnUserConnected;
                ChatService.OnUserDisconnected += ChatService_OnUserDisconnected;
                ChatService.OnMessageReceived += ChatService_OnMessageReceived;
                ChatService.OnMessagesHistoryReceived += ChatService_OnMessagesHistoryReceived;
                ChatService.OnConnectionClosed += ChatService_OnConnectionClosed;
            }
        }

        private void ChatService_OnMessageReceived(string sender, string message)
        {
            object listBoxItem;
            if(sender == User.UserName)
            {
                listBoxItem = new { Text = $"{sender}: {message}", MessageType = MessageTypes.Self };
            }
            else
            {
                listBoxItem = new { Text = $"{sender}: {message}", MessageType = MessageTypes.Other };
            }

            this.Dispatcher.Invoke(() =>
            {
                MessagesBox.Items.Add(listBoxItem);
            });
        }

        private void ChatService_OnUserConnected(string username)
        {
            this.Dispatcher.Invoke(() =>
            {
                MessagesBox.Items.Add(new { Text = $" - {username} has joined the chat - ", MessageType = MessageTypes.UserMoving });
            });
        }

        private void ChatService_OnUserDisconnected(string username)
        {
            this.Dispatcher.Invoke(() =>
            {
                MessagesBox.Items.Add(new { Text = $" - {username} has left the chat - ", MessageType = MessageTypes.UserMoving });
            });
        }


        private void ChatService_OnMessagesHistoryReceived(IEnumerable<string> messages)
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (var message in messages)
                {
                    MessagesBox.Items.Add(new { Text = message, MessageType = MessageTypes.HistoryMessage });
                }
            });
        }

        private void ChatService_OnConnectionClosed()
        {
            MessageBox.Show("Соединение с сервером потеряно!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.Dispatcher.Invoke(() =>
            {
                var enterForm = new EnterForm();
                enterForm.Show();
                this.Close();
            });
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ChatService?.SendMessage(User?.UserName, SendMessageBox.Text);
                SendMessageBox.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Возникла ошибка при отправке сообщения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GetHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ChatService?.GetMessageHistory();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Возникла ошибка при получении истории сообщений!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeChatEvents();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(ChatService != null)
                ChatService.StopConnection();
        }
    }
}
