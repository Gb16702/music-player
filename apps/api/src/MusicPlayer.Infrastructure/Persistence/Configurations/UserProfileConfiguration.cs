using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicPlayer.Domain.Users;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Persistence.Configurations
{
    internal sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("user_profiles");

            builder.HasKey(profile => profile.UserId);

            builder.Property(profile => profile.DisplayName)
                .HasMaxLength(UserProfile.MaxDisplayNameLength)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<UserProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
