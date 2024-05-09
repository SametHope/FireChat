using System.Windows;

namespace FireChat.Helpers
{
    public static class MessageBoxHelper
    {
        public static MessageBoxResult ShowSuccess(string message, string title = "Success")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult ShowWarning(string message, string title = "Warning")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static MessageBoxResult ShowError(string message, string title = "Error")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowQuestion(string message, string title = "Question")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

    }
}
