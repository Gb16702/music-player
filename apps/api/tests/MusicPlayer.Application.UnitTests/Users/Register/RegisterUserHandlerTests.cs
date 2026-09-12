using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.Register;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Application.UnitTests.Users.Register;

public sealed class RegisterUserHandlerTests
{
    [Fact]
    public async Task HandleAsyncReturnsUserIdWhenRegistrationSucceeds()
    {
        var userId = Guid.NewGuid();
        var handler = CreateHandler(
            new FakeIdentityService(Result<Guid>.Success(userId)),
            new FakeUserProfileRepository(),
            new FakeUnitOfWork());

        var result = await handler.HandleAsync(
            new RegisterUserCommand("user@example.com", "Password1!", "Jane Doe"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value);
    }

    [Fact]
    public async Task HandleAsyncReturnsIdentityErrorWhenUserCreationFails()
    {
        var identityError = RegistrationErrors.EmailAlreadyExists("Email is already taken.");
        var handler = CreateHandler(
            new FakeIdentityService(Result<Guid>.Failure(identityError)),
            new FakeUserProfileRepository(),
            new FakeUnitOfWork());

        var result = await handler.HandleAsync(
            new RegisterUserCommand("user@example.com", "Password1!", "Jane Doe"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(RegistrationErrors.EmailAlreadyExistsCode, result.Error!.Code);
        Assert.Equal("Email is already taken.", result.Error.Message);
    }

    [Fact]
    public async Task HandleAsyncReturnsInvalidDisplayNameWhenProfileIsInvalid()
    {
        var handler = CreateHandler(
            new FakeIdentityService(Result<Guid>.Success(Guid.NewGuid())),
            new FakeUserProfileRepository(),
            new FakeUnitOfWork());

        var result = await handler.HandleAsync(
            new RegisterUserCommand("user@example.com", "Password1!", " "),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(RegistrationErrors.InvalidDisplayNameCode, result.Error!.Code);
    }

    private static RegisterUserHandler CreateHandler(
        IIdentityService identityService,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork)
    {
        return new RegisterUserHandler(identityService, userProfileRepository, unitOfWork);
    }

    private sealed class FakeIdentityService(Result<Guid> result) : IIdentityService
    {
        public Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken)
        {
            return Task.FromResult(result);
        }
    }

    private sealed class FakeUserProfileRepository : IUserProfileRepository
    {
        public List<UserProfile> Profiles { get; } = [];

        public void Add(UserProfile profile)
        {
            Profiles.Add(profile);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }
    }
}
