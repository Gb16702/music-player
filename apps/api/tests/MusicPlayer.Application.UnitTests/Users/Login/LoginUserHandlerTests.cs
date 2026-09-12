using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.Login;

namespace MusicPlayer.Application.UnitTests.Users.Login;

public sealed class LoginUserHandlerTests
{
    [Fact]
    public async Task HandleAsyncReturnsUserIdWhenSignInSucceeds()
    {
        var userId = Guid.NewGuid();
        var handler = new LoginUserHandler(new FakeAuthSignInService(Result<Guid>.Success(userId)));

        var result = await handler.HandleAsync(
            new LoginUserCommand("user@example.com", "Password1!", true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenSignInFails()
    {
        var handler = new LoginUserHandler(
            new FakeAuthSignInService(Result<Guid>.Failure(LoginErrors.InvalidCredentials())));

        var result = await handler.HandleAsync(
            new LoginUserCommand("user@example.com", "WrongPassword1!", false),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(LoginErrors.InvalidCredentialsCode, result.Error!.Code);
    }

    private sealed class FakeAuthSignInService(Result<Guid> result) : IAuthSignInService
    {
        public Task<Result<Guid>> SignInAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken)
        {
            return Task.FromResult(result);
        }
    }
}
