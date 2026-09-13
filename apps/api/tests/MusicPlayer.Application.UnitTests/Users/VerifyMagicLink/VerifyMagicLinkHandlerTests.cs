using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.RequestMagicLink;
using MusicPlayer.Application.Users.VerifyMagicLink;

namespace MusicPlayer.Application.UnitTests.Users.VerifyMagicLink;

public sealed class VerifyMagicLinkHandlerTests
{
    [Fact]
    public async Task HandleAsyncReturnsUserIdWhenTokenIsValid()
    {
        var userId = Guid.NewGuid();
        var handler = new VerifyMagicLinkHandler(
            new FakeMagicLinkTokenService(Result<string>.Success("user@example.com")),
            new FakeMagicLinkSignInService(Result<Guid>.Success(userId)));

        var result = await handler.HandleAsync(new VerifyMagicLinkCommand("valid-token"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenTokenIsInvalid()
    {
        var handler = new VerifyMagicLinkHandler(
            new FakeMagicLinkTokenService(Result<string>.Failure(MagicLinkTokenErrors.InvalidToken())),
            new FakeMagicLinkSignInService(Result<Guid>.Success(Guid.NewGuid())));

        var result = await handler.HandleAsync(new VerifyMagicLinkCommand("invalid-token"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(VerifyMagicLinkErrors.InvalidTokenCode, result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenTokenIsExpired()
    {
        var handler = new VerifyMagicLinkHandler(
            new FakeMagicLinkTokenService(Result<string>.Failure(MagicLinkTokenErrors.ExpiredToken())),
            new FakeMagicLinkSignInService(Result<Guid>.Success(Guid.NewGuid())));

        var result = await handler.HandleAsync(new VerifyMagicLinkCommand("expired-token"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(VerifyMagicLinkErrors.ExpiredTokenCode, result.Error!.Code);
    }

    private sealed class FakeMagicLinkTokenService(Result<string> result) : IMagicLinkTokenService
    {
        public string GenerateToken(string email) => "token";

        public Result<string> ValidateToken(string token) => result;
    }

    private sealed class FakeMagicLinkSignInService(Result<Guid> result) : IMagicLinkSignInService
    {
        public Task<Result<Guid>> SignInAsync(string email, CancellationToken cancellationToken) => Task.FromResult(result);
    }
}
