namespace MusicPlayer.Application.Users.Register
{
    public sealed class RegisterUserCommand
    {
        public RegisterUserCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }

        public string Password { get; }
    }
}
