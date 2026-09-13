using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Users.GetCurrentUser;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Application.UnitTests.Users.GetCurrentUser;

public sealed class GetCurrentUserHandlerTests
{
    [Fact]
    public async Task HandleAsyncReturnsCurrentUserWhenProfileAndEmailExist()
    {
        var userId = Guid.NewGuid();
        var handler = new GetCurrentUserHandler(
            new FakeUserProfileRepository(new UserProfile(userId, "Jane Doe")),
            new FakeUserAccountReader("user@example.com"));

        var result = await handler.HandleAsync(new GetCurrentUserQuery(userId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value!.UserId);
        Assert.Equal("user@example.com", result.Value.Email);
        Assert.Equal("Jane Doe", result.Value.DisplayName);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenProfileIsMissing()
    {
        var handler = new GetCurrentUserHandler(
            new FakeUserProfileRepository(null),
            new FakeUserAccountReader("user@example.com"));

        var result = await handler.HandleAsync(new GetCurrentUserQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(GetCurrentUserErrors.UserNotFoundCode, result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenEmailIsMissing()
    {
        var userId = Guid.NewGuid();
        var handler = new GetCurrentUserHandler(
            new FakeUserProfileRepository(new UserProfile(userId, "Jane Doe")),
            new FakeUserAccountReader(null));

        var result = await handler.HandleAsync(new GetCurrentUserQuery(userId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(GetCurrentUserErrors.UserNotFoundCode, result.Error!.Code);
    }

    private sealed class FakeUserProfileRepository(UserProfile? profile) : IUserProfileRepository
    {
        public void Add(UserProfile profile)
        {
        }

        public Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(profile is not null && profile.UserId == userId ? profile : null);
        }
    }

    private sealed class FakeUserAccountReader(string? email) : IUserAccountReader
    {
        public Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(email);
        }
    }
}
