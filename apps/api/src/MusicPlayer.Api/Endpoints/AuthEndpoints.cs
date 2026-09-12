using Microsoft.AspNetCore.Http.HttpResults;
using MusicPlayer.Api.Contracts;
using MusicPlayer.Api.Validation;
using MusicPlayer.Application.Users.Login;
using MusicPlayer.Application.Users.Register;

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

        var command = new RegisterUserCommand(
            request.Email.Trim(),
            request.Password,
            request.DisplayName);

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
}
