namespace MusicPlayer.Api.Contracts;

public sealed record CurrentUserResponse(Guid UserId, string Email, string DisplayName);
