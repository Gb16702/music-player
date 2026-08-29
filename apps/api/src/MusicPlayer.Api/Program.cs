using MusicPlayer.Api.Contracts;
using Scalar.AspNetCore;

const string LocalWebClientPolicy = "LocalWebClient";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy(LocalWebClientPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseCors(LocalWebClientPolicy);
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");

var api = app.MapGroup("/api/v1");

api.MapGet("/system/status", () =>
        TypedResults.Ok(new SystemStatusResponse("ok", DateTimeOffset.UtcNow)))
    .WithName("GetSystemStatus")
    .WithSummary("Returns the API availability status.")
    .WithDescription("Checks whether the API is available and running.")
    .WithTags("System");

app.Run();

public partial class Program;
