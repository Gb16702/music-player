using MusicPlayer.Application.Abstractions.Email;

namespace MusicPlayer.IntegrationTests;

public sealed class CapturingEmailSender : IEmailSender
{
    public string? LastRecipient { get; private set; }

    public string? LastMagicLinkUrl { get; private set; }

    public void Reset()
    {
        LastRecipient = null;
        LastMagicLinkUrl = null;
    }

    public Task SendMagicLinkAsync(string toEmail, string magicLinkUrl, CancellationToken cancellationToken)
    {
        LastRecipient = toEmail;
        LastMagicLinkUrl = magicLinkUrl;

        return Task.CompletedTask;
    }
}
