using FireChat.Database;
using FireChat.Helpers;
using FireChat.Models;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FireChat.Windows
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : MetroWindow
    {
        private ChatDatabase _database;
        private string _username;
        private string _nickname;

        public ChatWindow(ChatDatabase database, string username, string nickname)
        {
            InitializeComponent();
            _database = database;
            _username = username;
            _nickname = nickname;
        }

        private void ChatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = Title.Replace("{username}", _username);
            Title = Title.Replace("{nickname}", _nickname);
            StartListening();
        }

        private async void StartListening()
        {
            await _database.SetMessageAddedListener((messageModel) =>
            {
                ChatMessageModel msg = new ChatMessageModel(messageModel);
                chatListView.Invoke(delegate
                {
                    chatListView.Items.Add(msg);
                    // We should check if we are not scrolled up for UX but whatever
                    if (chatListView.Items.Count > 0)
                    {
                        chatListView.ScrollIntoView(chatListView.Items[chatListView.Items.Count - 1]);
                    }
                });
            });

        }


        protected override void OnClosing(CancelEventArgs e)
        {
            _database.RemoveMessageAddedListener();
            base.OnClosing(e);
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            GoToLoginWindow();
        }

        private void GoToLoginWindow()
        {
            var loginWindow = new LoginWindow(_database); ;
            loginWindow.Left = this.Left;
            loginWindow.Top = this.Top;
            loginWindow.Show();
            this.Close();
        }

        private async void deleteAccountButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var answer = MessageBoxHelper.ShowQuestion("Are you sure you want to delete your account?\nYour messages will not be deleted.");

            if (answer != MessageBoxResult.Yes)
            {
                return;
            }

            if (await _database.DeleteUserAsync(_username))
            {
                MessageBoxHelper.ShowSuccess("Account deleted successfully.");
                GoToLoginWindow();
            }
            else
            {
                MessageBoxHelper.ShowError("An error occurred while deleting your account.");
            }
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            await SendCurrentMessageAsync();
            IsEnabled = true;
            messageBox.Focus();
        }

        private async Task SendCurrentMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(messageBox.Text))
            {
                return;
            }

            var messageModel = new ChatMessageModel(_username, _nickname, messageBox.Text, DateTime.Now);
            messageModel.SenderNickname = anonymousCheckbox.IsChecked == true ? "Anonymous" : _nickname;

            if (!await _database.SendMessageAsync(messageModel))
            {
                MessageBoxHelper.ShowError("An error occurred while sending the message.");
                return;
            }

            messageBox.Text = string.Empty;
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsEnabled = false;
                await SendCurrentMessageAsync();
                IsEnabled = true;
                messageBox.Focus();
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
    }
}
