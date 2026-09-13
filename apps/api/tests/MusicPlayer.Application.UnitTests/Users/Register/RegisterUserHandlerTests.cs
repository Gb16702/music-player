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
        var unitOfWork = new FakeUnitOfWork();
        var handler = CreateHandler(
            new FakeIdentityService(Result<Guid>.Success(userId)),
            new FakeUserProfileRepository(),
            unitOfWork);

        var result = await handler.HandleAsync(
            new RegisterUserCommand("user@example.com", "Password1!", "Jane Doe"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value);
        Assert.True(unitOfWork.Transaction!.IsCommitted);
    }

    [Fact]
    public async Task HandleAsyncReturnsIdentityErrorWhenUserCreationFails()
    {
        var unitOfWork = new FakeUnitOfWork();
        var identityError = RegistrationErrors.EmailAlreadyExists("Email is already taken.");
        var handler = CreateHandler(
            new FakeIdentityService(Result<Guid>.Failure(identityError)),
            new FakeUserProfileRepository(),
            unitOfWork);

        var result = await handler.HandleAsync(
            new RegisterUserCommand("user@example.com", "Password1!", "Jane Doe"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(RegistrationErrors.EmailAlreadyExistsCode, result.Error!.Code);
        Assert.Equal("Email is already taken.", result.Error.Message);
        Assert.False(unitOfWork.Transaction!.IsCommitted);
    }

    [Fact]
    public async Task HandleAsyncReturnsInvalidDisplayNameWhenProfileIsInvalid()
    {
        var identityService = new FakeIdentityService(Result<Guid>.Success(Guid.NewGuid()));
        var handler = CreateHandler(
            identityService,
            new FakeUserProfileRepository(),
            new FakeUnitOfWork());

        var result = await handler.HandleAsync(
            new RegisterUserCommand("user@example.com", "Password1!", " "),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(RegistrationErrors.InvalidDisplayNameCode, result.Error!.Code);
        Assert.Equal(0, identityService.CallCount);
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
        public int CallCount { get; private set; }

        public Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken)
        {
            CallCount++;

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

        public Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Profiles.FirstOrDefault(profile => profile.UserId == userId));
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public FakeUnitOfWorkTransaction? Transaction { get; private set; }

        public Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            Transaction = new FakeUnitOfWorkTransaction();

            return Task.FromResult<IUnitOfWorkTransaction>(Transaction);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }
    }

    private sealed class FakeUnitOfWorkTransaction : IUnitOfWorkTransaction
    {
        public bool IsCommitted { get; private set; }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            IsCommitted = true;

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
