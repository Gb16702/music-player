using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.SignInWithExternalProvider;

namespace MusicPlayer.Application.UnitTests.Users.SignInWithExternalProvider;

public sealed class SignInWithExternalProviderHandlerTests
{
    [Fact]
    public async Task HandleAsyncReturnsUserIdWhenExternalSignInSucceeds()
    {
        var userId = Guid.NewGuid();
        var handler = new SignInWithExternalProviderHandler(
            new FakeExternalAuthSignInService(Result<Guid>.Success(userId)));

        var result = await handler.HandleAsync(
            new SignInWithExternalProviderCommand("user@example.com", "Google", "google-subject"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenExternalSignInFails()
    {
        var handler = new SignInWithExternalProviderHandler(
            new FakeExternalAuthSignInService(
                Result<Guid>.Failure(ExternalAuthErrors.SignInFailed("Sign-in failed."))));

        var result = await handler.HandleAsync(
            new SignInWithExternalProviderCommand("user@example.com", "Google", "google-subject"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ExternalAuthErrors.SignInFailedCode, result.Error!.Code);
    }

    private sealed class FakeExternalAuthSignInService(Result<Guid> result) : IExternalAuthSignInService
    {
        public Task<Result<Guid>> SignInAsync(
            string email,
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(result);
        }
    }
}
