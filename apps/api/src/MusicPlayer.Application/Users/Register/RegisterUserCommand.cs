namespace MusicPlayer.Application.Users.Register
{
    public sealed class RegisterUserCommand
    {
        public RegisterUserCommand(string email, string password, string displayName)
        {
            Email = email;
            Password = password;
            DisplayName = displayName;
        }

        public string Email { get; }

        public string Password { get; }

        public string DisplayName { get; }
    }
}
