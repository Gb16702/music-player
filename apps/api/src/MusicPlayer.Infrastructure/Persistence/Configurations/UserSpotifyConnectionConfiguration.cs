using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicPlayer.Domain.Users;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.Persistence.Configurations;

internal sealed class UserSpotifyConnectionConfiguration : IEntityTypeConfiguration<UserSpotifyConnection>
{
    public void Configure(EntityTypeBuilder<UserSpotifyConnection> builder)
    {
        builder.ToTable("user_spotify_connections");

        builder.HasKey(connection => connection.UserId);

        builder.Property(connection => connection.AccessToken)
            .HasMaxLength(4096)
            .IsRequired();

        builder.Property(connection => connection.RefreshToken)
            .HasMaxLength(4096)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<UserSpotifyConnection>(connection => connection.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
