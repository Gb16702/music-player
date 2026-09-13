namespace MusicPlayer.Application.Abstractions.Email
{
    public interface IEmailSender
    {
        Task SendMagicLinkAsync(string toEmail, string magicLinkUrl, CancellationToken cancellationToken);
    }
}
