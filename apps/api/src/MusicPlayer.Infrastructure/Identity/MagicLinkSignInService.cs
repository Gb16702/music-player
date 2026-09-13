using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.Register;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class MagicLinkSignInService : IMagicLinkSignInService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MagicLinkSignInService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserProfileRepository userProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> SignInAsync(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var normalizedEmail = email.Trim();
            var user = await _userManager.FindByEmailAsync(normalizedEmail);

            if (user is null)
            {
                var createUserResult = await CreateUserWithProfileAsync(normalizedEmail, cancellationToken);

                if (!createUserResult.IsSuccess)
                {
                    return createUserResult;
                }

                user = await _userManager.FindByIdAsync(createUserResult.Value!.ToString());

                if (user is null)
                {
                    return Result<Guid>.Failure(RegistrationErrors.RegistrationFailed("User account could not be created."));
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            return Result<Guid>.Success(user.Id);
        }

        private async Task<Result<Guid>> CreateUserWithProfileAsync(string email, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                return Result<Guid>.Failure(IdentityErrorMapper.Map(createResult.Errors));
            }

            _userProfileRepository.Add(new UserProfile(user.Id));

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}
