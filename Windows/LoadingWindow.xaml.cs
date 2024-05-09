using FireChat.Database;
using MahApps.Metro.Controls;

namespace FireChat.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoadingWindow : MetroWindow
    {
        private ChatDatabase _database;

        public LoadingWindow()
        {
            InitializeComponent();
            progressRing.Visibility = System.Windows.Visibility.Visible;
            errorPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private async void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _database = new ChatDatabase(
                path: @"https://avaloniachat-default-rtdb.europe-west1.firebasedatabase.app/",
                secret: @"V8FHtw0k2NuhVopRQX5NqRXwFYLGwcp2AT5aKOPQ",
                users: "Users",
                messages: "Messages");

            bool connectionSuccess = await _database.TestConnectionAsync();

            if (connectionSuccess)
            {
                GoToLoginWindow();
            }
            else
            {
                HandleConnectionFailure();
            }
        }

        private void GoToLoginWindow()
        {
            var loginWindow = new LoginWindow(_database);
            loginWindow.Left = this.Left;
            loginWindow.Top = this.Top;
            loginWindow.Show();
            this.Close();
        }

        private void HandleConnectionFailure()
        {
            progressRing.Visibility = System.Windows.Visibility.Collapsed;
            errorPanel.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
