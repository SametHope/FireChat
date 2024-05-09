using FireChat.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FireChat.Database
{
    public class ChatDatabase
    {
        private readonly string _users = "users";
        private readonly string _messages = "messages";

        private readonly FirebaseConfig _config;
        private readonly FirebaseClient _client;
        private EventStreamResponse _messageAddedStream;

        /// <summary>
        /// Creates a new instance of the FireDB class, initializing the connection to the database.
        /// </summary>
        public ChatDatabase(string path, string secret, string users, string messages)
        {
            _config = new FirebaseConfig
            {
                BasePath = path,
                AuthSecret = secret
            };

            _client = new FirebaseClient(_config);

            _users = users;
            _messages = messages;
        }

        public async Task<bool> TestConnectionAsync()
        {
            // If the connection fails, an exception will be thrown
            // this is an acceptable way to check the connection
            try
            {
                await _client.GetAsync("this string doesn't really matter");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                // If the user exists, the body will not be "null"
                return (await _client.GetAsync($"{_users}/{username}")).Body != "null";

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RegisterUserAsync(string username, UserModel userModel)
        {
            try
            {
                var result = await _client.SetAsync($"{_users}/{username}/", userModel);
                return result.StatusCode == System.Net.HttpStatusCode.OK;

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            try
            {
                var result = await _client.DeleteAsync($"{_users}/{username}");
                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetUserPasswordAsync(string username)
        {
            try
            {
                var result = await _client.GetAsync($"{_users}/{username}/{nameof(UserModel.Password)}");
                return result.ResultAs<string>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetUserNicknameAsync(string username)
        {
            try
            {
                var result = await _client.GetAsync($"{_users}/{username}/{nameof(UserModel.Nickname)}");
                return result.ResultAs<string>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<ChatMessageModel>> GetMessagesAsync(Func<ChatMessageModel, bool> predicate)
        {
            try
            {
                var messages = await _client.GetAsync($"{_messages}");
                var messagesDict = messages.ResultAs<Dictionary<string, ChatMessageModel>>();

                if (messagesDict == null)
                {
                    return null;
                }

                return messagesDict.Select(x => x.Value).Where(x => predicate(x));
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SendMessageAsync(ChatMessageModel chatMessageModel)
        {
            try
            {
                var result = await _client.PushAsync($"{_messages}", chatMessageModel);
                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteMessagesAsync(Func<ChatMessageModel, bool> predicate)
        {
            try
            {
                var messages = await _client.GetAsync($"{_messages}");
                var messagesDict = messages.ResultAs<Dictionary<string, ChatMessageModel>>();

                if (messagesDict == null)
                {
                    return true;
                }

                foreach (var message in messagesDict)
                {
                    if (predicate(message.Value))
                    {
                        await _client.DeleteAsync($"{_messages}/{message.Key}");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private ulong _messageParserCounter = 0;
        private ChatMessageModel _parseMessage = new ChatMessageModel();

        // Will also be invoked for old messages when the listener is set
        public async Task SetMessageAddedListener(Action<ChatMessageModel> onMessageAdded)
        {
            try
            {
                _messageAddedStream?.Dispose();
                _messageAddedStream = await _client.OnAsync($"{_messages}",
                    added: (sender, args, context) =>
                    {
                        if (_messageParserCounter % 4UL == 0)
                        {
                            _parseMessage.Content = args.Data;
                        }
                        else if (_messageParserCounter % 4UL == 1)
                        {
                            _parseMessage.SenderNickname = args.Data;
                        }
                        else if (_messageParserCounter % 4UL == 2)
                        {
                            _parseMessage.SenderUsername = args.Data;
                        }
                        else if (_messageParserCounter % 4UL == 3)
                        {
                            _parseMessage.TimeSent = DateTime.Parse(args.Data);
                            onMessageAdded?.Invoke(_parseMessage);
                        }
                        _messageParserCounter++;
                    });
            }
            catch
            {

            }

        }

        public void RemoveMessageAddedListener()
        {
            _messageAddedStream?.Dispose();
            _messageAddedStream = null;
        }
    }
}
