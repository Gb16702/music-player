using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MusicPlayer.Application.Abstractions.Email;

namespace MusicPlayer.Infrastructure.Email
{
    internal sealed class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _emailOptions;
        private readonly SmtpOptions _smtpOptions;
        private readonly MagicLinkOptions _magicLinkOptions;

        public SmtpEmailSender(
            IOptions<EmailOptions> emailOptions,
            IOptions<SmtpOptions> smtpOptions,
            IOptions<MagicLinkOptions> magicLinkOptions)
        {
            _emailOptions = emailOptions.Value;
            _smtpOptions = smtpOptions.Value;
            _magicLinkOptions = magicLinkOptions.Value;
        }

        public async Task SendMagicLinkAsync(string toEmail, string magicLinkUrl, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (subject, htmlBody) = MagicLinkEmailContent.Create(magicLinkUrl, _magicLinkOptions.TokenLifetimeMinutes);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailOptions.FromName, _emailOptions.FromAddress));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, MailKit.Security.SecureSocketOptions.Auto, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
