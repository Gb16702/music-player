using Microsoft.AspNetCore.Http.HttpResults;
using MusicPlayer.Api.Contracts;

namespace MusicPlayer.Api.Endpoints
{
    internal static class SystemEndpoints
    {
        public static RouteGroupBuilder MapSystemEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/system/status", GetSystemStatus)
                .WithName("GetSystemStatus")
                .WithSummary("Returns the API availability status.")
                .WithDescription("Checks whether the API is available and running.")
                .WithTags("System");

            return group;
        }

        private static Ok<SystemStatusResponse> GetSystemStatus()
        {
            var response = new SystemStatusResponse("ok", DateTimeOffset.UtcNow);

            return TypedResults.Ok(response);
        }
    }
}
