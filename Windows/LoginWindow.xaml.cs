using FireChat.Database;
using MahApps.Metro.Controls;

namespace FireChat.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        private ChatDatabase _database;
        public LoginWindow(ChatDatabase database)
        {
            InitializeComponent();
            _database = database;
        }

        private async void loginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            string username = usernameBox.Text;
            string password = passwordBox.Password;

            if (!await ChatDatabaseHelper.ValidateLoginAsync(username, password, _database))
            {
                return;
            }

            string nickname = await _database.GetUserNicknameAsync(username);

            GoToChatWindow(username, nickname);

        }

        private void registerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GoToRegisterWindow();
        }

        private void GoToRegisterWindow()
        {
            var registerWindow = new RegisterWindow(_database);
            registerWindow.Left = this.Left;
            registerWindow.Top = this.Top;
            registerWindow.Show();
            this.Close();
        }

        private void GoToChatWindow(string username, string nickname)
        {
            var chatWindow = new ChatWindow(_database, username, nickname);
            chatWindow.Left = this.Left + (this.Width - chatWindow.Width) / 2;
            chatWindow.Top = this.Top + (this.Height - chatWindow.Height) / 2;
            chatWindow.Show();
            this.Close();
        }
    }
}