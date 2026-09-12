using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Common;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Application.Users.Register
{
    public sealed class RegisterUserHandler : IRegisterUserHandler
    {
        private readonly IIdentityService _identityService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserHandler(IIdentityService identityService, IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            string normalizedDisplayName;

            try
            {
                normalizedDisplayName = UserProfile.NormalizeDisplayName(command.DisplayName);
            }
            catch (ArgumentException exception)
            {
                return Result<Guid>.Failure(RegistrationErrors.InvalidDisplayName(exception.Message));
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var createUserResult = await _identityService.CreateUserAsync(command.Email, command.Password, cancellationToken);

            if (!createUserResult.IsSuccess)
            {
                return Result<Guid>.Failure(createUserResult.Error!);
            }

            var userProfile = new UserProfile(createUserResult.Value!, normalizedDisplayName);

            _userProfileRepository.Add(userProfile);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result<Guid>.Success(createUserResult.Value!);
        }
    }
}
