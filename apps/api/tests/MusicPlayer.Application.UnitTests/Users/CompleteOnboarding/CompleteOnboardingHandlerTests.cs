using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Users.CompleteOnboarding;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Application.UnitTests.Users.CompleteOnboarding;

public sealed class CompleteOnboardingHandlerTests
{
    [Fact]
    public async Task HandleAsyncCompletesOnboardingWhenProfileIsPending()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeUserProfileRepository(new UserProfile(userId));
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CompleteOnboardingHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(
            new CompleteOnboardingCommand(userId, "Jane Doe", "https://example.com/avatar.png"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value!.UserId);
        Assert.Equal("Jane Doe", result.Value.DisplayName);
        Assert.Equal("https://example.com/avatar.png", result.Value.AvatarUrl);
        Assert.True(result.Value.OnboardingCompleted);
        Assert.True(repository.Profiles[0].OnboardingCompleted);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenProfileIsMissing()
    {
        var handler = new CompleteOnboardingHandler(new FakeUserProfileRepository(null), new FakeUnitOfWork());

        var result = await handler.HandleAsync(
            new CompleteOnboardingCommand(Guid.NewGuid(), "Jane Doe", null),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CompleteOnboardingErrors.ProfileNotFoundCode, result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenOnboardingAlreadyCompleted()
    {
        var userId = Guid.NewGuid();
        var profile = new UserProfile(userId);
        profile.CompleteOnboarding("Jane Doe", null);

        var handler = new CompleteOnboardingHandler(new FakeUserProfileRepository(profile), new FakeUnitOfWork());

        var result = await handler.HandleAsync(
            new CompleteOnboardingCommand(userId, "Another Name", null),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CompleteOnboardingErrors.AlreadyCompletedCode, result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsyncReturnsFailureWhenDisplayNameIsInvalid()
    {
        var userId = Guid.NewGuid();
        var handler = new CompleteOnboardingHandler(
            new FakeUserProfileRepository(new UserProfile(userId)),
            new FakeUnitOfWork());

        var result = await handler.HandleAsync(
            new CompleteOnboardingCommand(userId, " ", null),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CompleteOnboardingErrors.InvalidDisplayNameCode, result.Error!.Code);
    }

    private sealed class FakeUserProfileRepository(UserProfile? profile) : IUserProfileRepository
    {
        public List<UserProfile> Profiles { get; } = profile is null ? [] : [profile];

        public void Add(UserProfile profile)
        {
            Profiles.Add(profile);
        }

        public Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Profiles.FirstOrDefault(item => item.UserId == userId));
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;

            return Task.FromResult(1);
        }
    }
}
