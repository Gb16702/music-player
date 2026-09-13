using MusicPlayer.Application.Abstractions.Email;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Users.RequestMagicLink;

namespace MusicPlayer.Application.UnitTests.Users.RequestMagicLink;

public sealed class RequestMagicLinkHandlerTests
{
    [Fact]
    public async Task HandleAsyncSendsMagicLinkEmail()
    {
        var emailSender = new FakeEmailSender();
        var handler = new RequestMagicLinkHandler(
            new FakeMagicLinkTokenService("token-123"),
            new FakeMagicLinkUrlBuilder("http://localhost:3000/auth/magic-link?token=token-123"),
            emailSender);

        var result = await handler.HandleAsync(
            new RequestMagicLinkCommand("user@example.com"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", emailSender.LastRecipient);
        Assert.Equal("http://localhost:3000/auth/magic-link?token=token-123", emailSender.LastMagicLinkUrl);
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        public string? LastRecipient { get; private set; }

        public string? LastMagicLinkUrl { get; private set; }

        public Task SendMagicLinkAsync(string toEmail, string magicLinkUrl, CancellationToken cancellationToken)
        {
            LastRecipient = toEmail;
            LastMagicLinkUrl = magicLinkUrl;

            return Task.CompletedTask;
        }
    }

    private sealed class FakeMagicLinkTokenService(string token) : IMagicLinkTokenService
    {
        public string GenerateToken(string email) => token;

        public Application.Common.Result<string> ValidateToken(string token) =>
            Application.Common.Result<string>.Success("user@example.com");
    }

    private sealed class FakeMagicLinkUrlBuilder(string url) : IMagicLinkUrlBuilder
    {
        public string BuildVerifyUrl(string token) => url;
    }
}
