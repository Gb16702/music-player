namespace MusicPlayer.Infrastructure.Email
{
    internal static class MagicLinkEmailContent
    {
        public static (string Subject, string HtmlBody) Create(string magicLinkUrl, int tokenLifetimeMinutes)
        {
            var subject = "Your Music Player sign-in link";

            var htmlBody = $"""
                <!DOCTYPE html>
                <html lang="en">
                <body style="font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; background-color: #111; color: #f5f5f5; padding: 32px;">
                  <div style="max-width: 480px; margin: 0 auto; background: #1c1c1e; border-radius: 12px; padding: 32px;">
                    <h1 style="margin-top: 0; font-size: 24px;">Sign in to Music Player</h1>
                    <p style="line-height: 1.5; color: #d1d1d6;">Click the button below to sign in. This link expires in {tokenLifetimeMinutes} minutes.</p>
                    <p style="margin: 32px 0;">
                      <a href="{magicLinkUrl}" style="display: inline-block; background: #fa2d48; color: #fff; text-decoration: none; padding: 12px 20px; border-radius: 999px; font-weight: 600;">Sign in</a>
                    </p>
                    <p style="font-size: 13px; color: #8e8e93;">If you did not request this email, you can ignore it.</p>
                  </div>
                </body>
                </html>
                """;

            return (subject, htmlBody);
        }
    }
}
