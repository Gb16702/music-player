using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
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

        public async Task<Guid> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var userId = await _identityService.CreateUserAsync(command.Email, command.Password, cancellationToken);

            var userProfile = new UserProfile(userId, command.DisplayName);

            _userProfileRepository.Add(userProfile);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return userId;
        }
    }
}
