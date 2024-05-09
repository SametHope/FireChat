using FireChat.Database;
using FireChat.Helpers;
using MahApps.Metro.Controls;

namespace FireChat.Windows
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : MetroWindow
    {
        private ChatDatabase _database;
        public RegisterWindow(ChatDatabase database)
        {
            InitializeComponent();
            _database = database;
        }

        private async void registerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.IsEnabled = false;
            string nickname = nicknameBox.Text;
            string username = usernameBox.Text;
            string password1 = passwordBox1.Password;
            string password2 = passwordBox2.Password;

            if (!await ChatDatabaseHelper.ValidateRegisterAsync(_database, nickname, username, password1, password2))
            {
                this.IsEnabled = true;
                return;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password1);
            var newUser = new Models.UserModel(nickname, hashedPassword);

            bool success = await _database.RegisterUserAsync(username, newUser);
            if (!success)
            {
                this.IsEnabled = true;
                MessageBoxHelper.ShowError("An error occurred while registering. Please try again.");
                return;
            }

            MessageBoxHelper.ShowSuccess("Registration successful.");
            this.IsEnabled = true;
        }

        private void backButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenLoginWindow();
        }

        private void OpenLoginWindow()
        {
            var loginWindow = new LoginWindow(_database);
            loginWindow.Left = this.Left;
            loginWindow.Top = this.Top;
            loginWindow.Show();
        }
    }
}
