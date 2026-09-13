using MusicPlayer.Domain.Users;

namespace MusicPlayer.Domain.UnitTests.Users
{
    public sealed class UserProfileTests
    {
        [Fact]
        public void ConstructorCreatesPendingProfile()
        {
            var userId = Guid.NewGuid();

            var profile = new UserProfile(userId);

            Assert.Equal(userId, profile.UserId);
            Assert.Null(profile.DisplayName);
            Assert.Null(profile.AvatarUrl);
            Assert.False(profile.OnboardingCompleted);
        }

        [Fact]
        public void ConstructorRejectsEmptyUserId()
        {
            var emptyUserId = Guid.Empty;

            var exception = Assert.Throws<ArgumentException>(() => new UserProfile(emptyUserId));

            Assert.Equal("userId", exception.ParamName);
        }

        [Fact]
        public void CompleteOnboardingTrimsDisplayNameAndSetsAvatar()
        {
            var profile = new UserProfile(Guid.NewGuid());

            profile.CompleteOnboarding(" John Doe ", " https://example.com/avatar.png ");

            Assert.Equal("John Doe", profile.DisplayName);
            Assert.Equal("https://example.com/avatar.png", profile.AvatarUrl);
            Assert.True(profile.OnboardingCompleted);
        }

        [Fact]
        public void CompleteOnboardingAllowsMissingAvatar()
        {
            var profile = new UserProfile(Guid.NewGuid());

            profile.CompleteOnboarding("John Doe", null);

            Assert.Equal("John Doe", profile.DisplayName);
            Assert.Null(profile.AvatarUrl);
            Assert.True(profile.OnboardingCompleted);
        }

        [Fact]
        public void CompleteOnboardingRejectsInvalidDisplayName()
        {
            var profile = new UserProfile(Guid.NewGuid());

            var exception = Assert.Throws<ArgumentException>(() => profile.CompleteOnboarding(" ", null));

            Assert.Equal("displayName", exception.ParamName);
            Assert.False(profile.OnboardingCompleted);
        }

        [Fact]
        public void CompleteOnboardingRejectsWhenAlreadyCompleted()
        {
            var profile = new UserProfile(Guid.NewGuid());
            profile.CompleteOnboarding("John Doe", null);

            Assert.Throws<InvalidOperationException>(() => profile.CompleteOnboarding("Jane Doe", null));
        }

        [Fact]
        public void ChangeDisplayNameTrimsValue()
        {
            var profile = new UserProfile(Guid.NewGuid());
            profile.CompleteOnboarding("John Doe", null);

            profile.ChangeDisplayName(" Jane Doe ");

            Assert.Equal("Jane Doe", profile.DisplayName);
        }

        [Fact]
        public void ChangeDisplayNameKeepsCurrentValueWhenNewValueIsInvalid()
        {
            var profile = new UserProfile(Guid.NewGuid());
            profile.CompleteOnboarding("John Doe", null);

            var exception = Assert.Throws<ArgumentException>(() => profile.ChangeDisplayName(" "));

            Assert.Equal("displayName", exception.ParamName);
            Assert.Equal("John Doe", profile.DisplayName);
        }
    }
}
