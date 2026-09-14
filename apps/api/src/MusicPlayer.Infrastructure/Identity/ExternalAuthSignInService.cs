using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Abstractions.Identity;
using MusicPlayer.Application.Abstractions.Persistence;
using MusicPlayer.Application.Common;
using MusicPlayer.Application.Users.Register;
using MusicPlayer.Application.Users.SignInWithExternalProvider;
using MusicPlayer.Domain.Users;

namespace MusicPlayer.Infrastructure.Identity
{
    internal sealed class ExternalAuthSignInService : IExternalAuthSignInService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExternalAuthSignInService(
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

        public async Task<Result<Guid>> SignInAsync(
            string email,
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var normalizedEmail = email.Trim();
            var externalSignInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider,
                providerKey,
                isPersistent: true);

            if (externalSignInResult.Succeeded)
            {
                var linkedUser = await _userManager.FindByLoginAsync(loginProvider, providerKey);

                if (linkedUser is null)
                {
                    return Result<Guid>.Failure(
                        ExternalAuthErrors.SignInFailed("Linked user account could not be found."));
                }

                return Result<Guid>.Success(linkedUser.Id);
            }

            if (externalSignInResult.IsLockedOut)
            {
                return Result<Guid>.Failure(ExternalAuthErrors.SignInFailed("User account is locked out."));
            }

            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);

            if (existingUser is not null)
            {
                return await LinkExternalLoginAndSignInAsync(existingUser, loginProvider, providerKey, cancellationToken);
            }

            return await CreateUserWithExternalLoginAsync(normalizedEmail, loginProvider, providerKey, cancellationToken);
        }

        private async Task<Result<Guid>> LinkExternalLoginAndSignInAsync(
            ApplicationUser user,
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken)
        {
            var addLoginResult = await _userManager.AddLoginAsync(
                user,
                new UserLoginInfo(loginProvider, providerKey, loginProvider));

            if (!addLoginResult.Succeeded)
            {
                return Result<Guid>.Failure(IdentityErrorMapper.Map(addLoginResult.Errors));
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            return Result<Guid>.Success(user.Id);
        }

        private async Task<Result<Guid>> CreateUserWithExternalLoginAsync(
            string email,
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken)
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

            var addLoginResult = await _userManager.AddLoginAsync(
                user,
                new UserLoginInfo(loginProvider, providerKey, loginProvider));

            if (!addLoginResult.Succeeded)
            {
                return Result<Guid>.Failure(IdentityErrorMapper.Map(addLoginResult.Errors));
            }

            _userProfileRepository.Add(new UserProfile(user.Id));

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _signInManager.SignInAsync(user, isPersistent: true);

            return Result<Guid>.Success(user.Id);
        }
    }
}
