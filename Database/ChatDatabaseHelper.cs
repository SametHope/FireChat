using FireChat.Helpers;
using System.Threading.Tasks;

namespace FireChat.Database
{
    public static class ChatDatabaseHelper
    {
        public static async Task<bool> ValidateRegisterAsync(ChatDatabase database, string nickname, string username, string password1, string password2)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                MessageBoxHelper.ShowWarning("Nickname field cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBoxHelper.ShowWarning("Username field cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(password1) || string.IsNullOrWhiteSpace(password2))
            {
                MessageBoxHelper.ShowWarning("Password fields cannot be empty.");
                return false;
            }

            if (username.Length < 3)
            {
                MessageBoxHelper.ShowWarning("Username must be at least 3 characters long.");
                return false;
            }

            if (password1 != password2)
            {
                MessageBoxHelper.ShowWarning("Passwords do not match.");
                return false;
            }

            if (await database.UserExistsAsync(username))
            {
                MessageBoxHelper.ShowWarning("A user with the given username already exists.");
                return false;
            }

            return true;
        }

        public static async Task<bool> ValidateLoginAsync(string username, string password, ChatDatabase database)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBoxHelper.ShowWarning("Username field cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBoxHelper.ShowWarning("Password field cannot be empty.");
                return false;
            }

            if (!await database.UserExistsAsync(username))
            {
                MessageBoxHelper.ShowWarning("A user with the given username does not exists.");
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, await database.GetUserPasswordAsync(username)))
            {
                MessageBoxHelper.ShowWarning("Incorrect password.");
                return false;
            }

            return true;
        }
    }
}
