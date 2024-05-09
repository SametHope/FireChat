using System;

namespace FireChat.Models
{
    public class ChatMessageModel
    {
        public string Content { get; set; }
        public string SenderNickname { get; set; }
        public string SenderUsername { get; set; }
        public DateTime TimeSent { get; set; }

        public ChatMessageModel()
        {

        }

        public ChatMessageModel(string senderUsername, string senderNickname, string content, DateTime timeSent)
        {
            SenderUsername = senderUsername;
            SenderNickname = senderNickname;
            Content = content;
            TimeSent = timeSent;
        }

        public ChatMessageModel(ChatMessageModel other)
        {
            SenderUsername = other.SenderUsername;
            SenderNickname = other.SenderNickname;
            Content = other.Content;
            TimeSent = other.TimeSent;
        }

        public string ToChatString()
        {
            return $"{TimeSent:dd.MM.yyyy HH:mm:ss} - {SenderNickname}: {Content}";
        }
    }
}