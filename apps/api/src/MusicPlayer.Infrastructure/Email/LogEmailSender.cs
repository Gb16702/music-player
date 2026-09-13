using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicPlayer.Application.Abstractions.Email;

namespace MusicPlayer.Infrastructure.Email
{
    internal sealed class LogEmailSender : IEmailSender
    {
        private readonly ILogger<LogEmailSender> _logger;
        private readonly MagicLinkOptions _magicLinkOptions;

        public LogEmailSender(ILogger<LogEmailSender> logger, IOptions<MagicLinkOptions> magicLinkOptions)
        {
            _logger = logger;
            _magicLinkOptions = magicLinkOptions.Value;
        }

        public Task SendMagicLinkAsync(string toEmail, string magicLinkUrl, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (subject, htmlBody) = MagicLinkEmailContent.Create(magicLinkUrl, _magicLinkOptions.TokenLifetimeMinutes);

            _logger.LogInformation(
                "Magic link email captured for {Recipient}. Subject: {Subject}. Link: {MagicLinkUrl}. Html length: {HtmlLength}",
                toEmail,
                subject,
                magicLinkUrl,
                htmlBody.Length);

            return Task.CompletedTask;
        }
    }
}
