namespace FireChat.Models
{
    public class UserModel
    {
        public string Nickname { get; set; }
        public string Password { get; set; }

        public UserModel(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }

        public UserModel()
        {

        }
    }
}
