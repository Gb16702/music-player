namespace MusicPlayer.Infrastructure.Google
{
    internal static class GoogleAuthConstants
    {
        public const string LoginProvider = "Google";

        /// <summary>Google redirect URI target — handled by OAuth middleware only (not a normal page).</summary>
        public const string OAuthCallbackPath = "/signin-google";

        /// <summary>App route that finishes sign-in after middleware has validated Google.</summary>
        public const string ApplicationCallbackPath = "/api/v1/auth/google/callback";
    }
}
