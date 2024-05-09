using FireChat.Database;
using FireChat.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsoleApp.MyTesting
{
    internal class DatabaseTester
    {
        private ChatDatabase db;
        public DatabaseTester(string path, string secret, string users, string messages)
        {
            db = new ChatDatabase(path, secret, users, messages);
        }

        public async Task RunTests(uint testAmount = 2)
        {
            Random random = new Random();
            _extraLogType = LogType.Failure;

            for (int i = 0; i < testAmount; i++)
            {
                // Create random username
                string username = $"TestUser{random.Next(1000)}";
                // Create username dependent password
                string password = $"epic pass {random.Next(1000)}";
                // Create username dependent nickname
                string nickname = $"{username} Nick{random.Next(1000)}";

                await RunTest(username, password, nickname);
            }

        }

        private async Task RunTest(string username, string password, string nickname)
        {
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync($"--- Testing for '{username}' && '{password}' && '{nickname}' ---");
            password = BCrypt.Net.BCrypt.HashPassword(password);

            Expect(nameof(db.UserExistsAsync), await db.UserExistsAsync(username), false);
            Expect(nameof(db.GetUserPasswordAsync), await db.GetUserPasswordAsync(username), null);

            Expect(nameof(db.RegisterUserAsync), await db.RegisterUserAsync(username, new UserModel(nickname, password)), true);

            Expect(nameof(db.UserExistsAsync), await db.UserExistsAsync(username), true);
            Expect(nameof(db.GetUserPasswordAsync), await db.GetUserPasswordAsync(username), password);

            Expect(nameof(db.DeleteUserAsync), await db.DeleteUserAsync(username), true);

            Expect(nameof(db.UserExistsAsync), await db.UserExistsAsync(username), false);
            Expect(nameof(db.GetUserPasswordAsync), await db.GetUserPasswordAsync(username), null);

            await db.SetMessageAddedListener(async newMessage =>
            {
                //await Console.Out.WriteLineAsync(newMessage.ToChatString());
            });

            for (int i = 0; i < 3; i++)
            {
                Expect(nameof(db.SendMessageAsync), await db.SendMessageAsync(new ChatMessageModel(username, nickname, "Epic message " + i, DateTime.Now)), true);
            }

            Expect(nameof(db.GetMessagesAsync), (await db.GetMessagesAsync(p => p.Content == "Epic message " + 0)).Count(), 1);

            Expect(nameof(db.DeleteMessagesAsync), await db.DeleteMessagesAsync((msg) => true), true);
            Expect(nameof(db.DeleteMessagesAsync), await db.DeleteMessagesAsync(msg => true), true);

            Expect(nameof(db.GetMessagesAsync), await db.GetMessagesAsync(p => true), null);

            db.RemoveMessageAddedListener();
        }


        private static LogType _extraLogType = LogType.Failure;
        private static void Expect(string testName, object result, object expectation)
        {
            bool testPassed = result?.Equals(expectation) ?? (expectation == null);
            string testResult = testPassed ? "Passed" : "Failed";

            Console.WriteLine($"Test Name: {testName} -> {testResult}");
            if ((_extraLogType == LogType.Failure && !testPassed) || (_extraLogType == LogType.Success && testPassed) || _extraLogType == LogType.Both)
            {
                Console.WriteLine($"Expected '{expectation ?? "null"}' but got '{result ?? "null"}'");
            }
        }


        private enum LogType
        {
            Success,
            Failure,
            Both
        }
    }
}
