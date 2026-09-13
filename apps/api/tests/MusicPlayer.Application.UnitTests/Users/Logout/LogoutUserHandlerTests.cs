using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Users.Logout;

namespace MusicPlayer.Application.UnitTests.Users.Logout;

public sealed class LogoutUserHandlerTests
{
    [Fact]
    public async Task HandleAsyncCallsSignOutService()
    {
        var signOutService = new FakeAuthSignOutService();
        var handler = new LogoutUserHandler(signOutService);

        await handler.HandleAsync(CancellationToken.None);

        Assert.Equal(1, signOutService.CallCount);
    }

    private sealed class FakeAuthSignOutService : IAuthSignOutService
    {
        public int CallCount { get; private set; }

        public Task SignOutAsync(CancellationToken cancellationToken)
        {
            CallCount++;

            return Task.CompletedTask;
        }
    }
}
