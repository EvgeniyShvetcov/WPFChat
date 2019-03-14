using ChatClient.Models;
using ChatClient.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ChatClient.Views
{
    /// <summary>
    /// Логика взаимодействия для EnterForm.xaml
    /// </summary>
    public partial class EnterForm : Window
    {
        public EnterForm()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            var connectionPath = ConnectionPathBox.Text;
            var username = UsernameBox.Text;
            if ((string.IsNullOrEmpty(connectionPath) || string.IsNullOrWhiteSpace(connectionPath))
                || (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username)))
            {
                MessageBox.Show("Необходимо заполнить все поля", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IDictionary<string, string> queryParameters = new Dictionary<string, string>
            {
                ["username"] = username
            };

            try
            {
                ChatConnectionService chatService = new ChatConnectionService(connectionPath, queryParameters);
                await chatService.StartConnection();
                if (chatService.IsConnected)
                {
                    MainWindow window = new MainWindow
                    {
                        User = new User { UserName = username },
                        ChatService = chatService
                    };
                    window.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось подключиться к чату. Попробуйте повторить позднее.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Не удалось подключиться к чату. Попробуйте повторить позднее.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
