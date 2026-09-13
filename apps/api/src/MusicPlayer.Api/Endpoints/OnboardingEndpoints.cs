using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicPlayer.Api.Contracts;
using MusicPlayer.Api.Validation;
using MusicPlayer.Application.Users.CompleteOnboarding;

namespace MusicPlayer.Api.Endpoints;

internal static class OnboardingEndpoints
{
    public static RouteGroupBuilder MapOnboardingEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/onboarding/complete", CompleteOnboarding)
            .RequireAuthorization()
            .WithName("CompleteOnboarding")
            .WithSummary("Completes user onboarding.")
            .WithDescription("Sets the display name and optional avatar, then marks onboarding as completed.")
            .WithTags("Onboarding");

        return group;
    }

    private static async Task<Results<Ok<CompleteOnboardingResponse>, Conflict<AuthErrorResponse>, NotFound<AuthErrorResponse>, BadRequest<AuthErrorResponse>, ValidationProblem, UnauthorizedHttpResult>> CompleteOnboarding(
        ClaimsPrincipal user,
        CompleteOnboardingRequest request,
        ICompleteOnboardingHandler handler,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(user, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var validationErrors = CompleteOnboardingRequestValidator.Validate(request);

        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        var command = new CompleteOnboardingCommand(userId, request.DisplayName.Trim(), request.AvatarUrl?.Trim());
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsSuccess)
        {
            var profile = result.Value!;

            return TypedResults.Ok(
                new CompleteOnboardingResponse(
                    profile.UserId,
                    profile.DisplayName,
                    profile.AvatarUrl,
                    profile.OnboardingCompleted));
        }

        var response = new AuthErrorResponse(result.Error!.Code, result.Error.Message);

        return result.Error.Code switch
        {
            CompleteOnboardingErrors.AlreadyCompletedCode => TypedResults.Conflict(response),
            CompleteOnboardingErrors.ProfileNotFoundCode => TypedResults.NotFound(response),
            _ => TypedResults.BadRequest(response),
        };
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out userId);
    }
}
