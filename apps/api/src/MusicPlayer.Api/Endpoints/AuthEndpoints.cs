using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicPlayer.Api.Contracts;
using MusicPlayer.Api.Validation;
using MusicPlayer.Application.Users.GetCurrentUser;
using MusicPlayer.Application.Users.Login;
using MusicPlayer.Application.Users.Logout;
using MusicPlayer.Application.Users.Register;
using MusicPlayer.Application.Users.RequestMagicLink;
using MusicPlayer.Application.Users.VerifyMagicLink;

namespace MusicPlayer.Api.Endpoints;

internal static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/auth/register", Register)
            .WithName("RegisterUser")
            .WithSummary("Registers a new local user account.")
            .WithDescription("Creates an application-owned account and profile.")
            .WithTags("Auth");

        group.MapPost("/auth/login", Login)
            .WithName("LoginUser")
            .WithSummary("Logs in with email and password.")
            .WithDescription("Creates an authentication cookie when credentials are valid.")
            .WithTags("Auth");

        group.MapPost("/auth/logout", Logout)
            .WithName("LogoutUser")
            .WithSummary("Logs out the current session.")
            .WithDescription("Clears the authentication cookie.")
            .WithTags("Auth");

        group.MapGet("/auth/me", GetCurrentUser)
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Returns the authenticated user profile.")
            .WithDescription("Requires a valid authentication cookie.")
            .WithTags("Auth");

        group.MapPost("/auth/magic-link/request", RequestMagicLink)
            .WithName("RequestMagicLink")
            .WithSummary("Sends a magic link sign-in email.")
            .WithDescription("Always returns success for valid email requests to avoid account enumeration.")
            .WithTags("Auth");

        group.MapPost("/auth/magic-link/verify", VerifyMagicLink)
            .WithName("VerifyMagicLink")
            .WithSummary("Verifies a magic link token and creates a session.")
            .WithDescription("Creates an account when the email is new, then signs the user in with a cookie.")
            .WithTags("Auth");

        return group;
    }

    private static async Task<Results<Created<RegisterResponse>, Conflict<AuthErrorResponse>, BadRequest<AuthErrorResponse>, ValidationProblem>> Register(
        RegisterRequest request,
        IRegisterUserHandler handler,
        CancellationToken cancellationToken)
    {
        var validationErrors = RegisterRequestValidator.Validate(request);

        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        var command = new RegisterUserCommand(request.Email.Trim(), request.Password);

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Created((string?)null, new RegisterResponse(result.Value!));
        }

        var response = new AuthErrorResponse(result.Error!.Code, result.Error.Message);

        if (result.Error.Code == RegistrationErrors.EmailAlreadyExistsCode)
        {
            return TypedResults.Conflict(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, ValidationProblem>> Login(
        LoginRequest request,
        ILoginUserHandler handler,
        CancellationToken cancellationToken)
    {
        var validationErrors = LoginRequestValidator.Validate(request);

        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        var command = new LoginUserCommand(request.Email.Trim(), request.Password, request.RememberMe);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(new LoginResponse(result.Value!));
        }

        return TypedResults.Unauthorized();
    }

    private static async Task<NoContent> Logout(ILogoutUserHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<CurrentUserResponse>, NotFound<AuthErrorResponse>, UnauthorizedHttpResult>> GetCurrentUser(
        ClaimsPrincipal user,
        IGetCurrentUserHandler handler,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(user, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await handler.HandleAsync(new GetCurrentUserQuery(userId), cancellationToken);

        if (result.IsSuccess)
        {
            var currentUser = result.Value!;

            return TypedResults.Ok(
                new CurrentUserResponse(
                    currentUser.UserId,
                    currentUser.Email,
                    currentUser.DisplayName,
                    currentUser.AvatarUrl,
                    currentUser.OnboardingCompleted));
        }

        return TypedResults.NotFound(new AuthErrorResponse(result.Error!.Code, result.Error.Message));
    }

    private static async Task<Results<NoContent, ValidationProblem>> RequestMagicLink(
        MagicLinkRequest request,
        IRequestMagicLinkHandler handler,
        CancellationToken cancellationToken)
    {
        var validationErrors = MagicLinkRequestValidator.Validate(request);

        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        await handler.HandleAsync(new RequestMagicLinkCommand(request.Email.Trim()), cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<LoginResponse>, BadRequest<AuthErrorResponse>, ValidationProblem>> VerifyMagicLink(
        MagicLinkVerifyRequest request,
        IVerifyMagicLinkHandler handler,
        CancellationToken cancellationToken)
    {
        var validationErrors = MagicLinkVerifyRequestValidator.Validate(request);

        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        var result = await handler.HandleAsync(new VerifyMagicLinkCommand(request.Token), cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(new LoginResponse(result.Value!));
        }

        return TypedResults.BadRequest(new AuthErrorResponse(result.Error!.Code, result.Error.Message));
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out userId);
    }
}
