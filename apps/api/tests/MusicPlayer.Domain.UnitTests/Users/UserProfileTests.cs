using MusicPlayer.Domain.Users;

namespace MusicPlayer.Domain.UnitTests.Users
{
    public sealed class UserProfileTests
    {
        [Fact]
        public void ConstructorTrimsDisplayName()
        {
            var userId = Guid.NewGuid();

            var profile = new UserProfile(userId, " John Doe ");

            Assert.Equal("John Doe", profile.DisplayName);
        }

        [Fact]
        public void ConstructorRejectsEmptyUserId()
        {
            var emptyUserId = Guid.Empty;

            var exception = Assert.Throws<ArgumentException>(() => new UserProfile(emptyUserId, "John Doe"));

            Assert.Equal("userId", exception.ParamName);
        }

        [Fact]
        public void ChangeDisplayNameTrimsValue()
        {
            var profile = new UserProfile(Guid.NewGuid(), "John Doe");

            profile.ChangeDisplayName(" Jane Doe ");

            Assert.Equal("Jane Doe", profile.DisplayName);
        }

        [Fact]
        public void ChangeDisplayNameKeepsCurrentValueWhenNewValueIsInvalid()
        {
            var profile = new UserProfile(Guid.NewGuid(), "John Doe");

            var exception = Assert.Throws<ArgumentException>(() => profile.ChangeDisplayName(" "));

            Assert.Equal("displayName", exception.ParamName);
            Assert.Equal("John Doe", profile.DisplayName);
        }
    }
}
