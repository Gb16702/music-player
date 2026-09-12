namespace MusicPlayer.Application.Users.Login
{
    public sealed class LoginUserCommand
    {
        public LoginUserCommand(string email, string password, bool isPersistent)
        {
            Email = email;
            Password = password;
            IsPersistent = isPersistent;
        }

        public string Email { get; }

        public string Password { get; }

        public bool IsPersistent { get; }
    }
}
