using OpenAI;
using OpenAI.Responses;
using System.ClientModel;
using DotNetEnv;

Env.Load();

string endpoint = Environment.GetEnvironmentVariable("FOUNDRY_ENDPOINT")
    ?? throw new InvalidOperationException("FOUNDRY_ENDPOINT saknas");

string apiKey = Environment.GetEnvironmentVariable("FOUNDRY_API_KEY")
    ?? throw new InvalidOperationException("FOUNDRY_API_KEY saknas");

string deploymentName = "gpt-5.4-mini";
string openAiEndpoint = endpoint.TrimEnd('/');
if (!openAiEndpoint.EndsWith("/openai/v1", StringComparison.OrdinalIgnoreCase))
{
    openAiEndpoint += "/openai/v1";
}

ResponsesClient client = new(
    model: deploymentName,
    credential: new ApiKeyCredential(apiKey),
    options: new OpenAIClientOptions
    {
        Endpoint = new Uri(openAiEndpoint)
    });

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/chat", (ChatRequest request, ILogger<Program> logger) =>
{
    try
    {
        CreateResponseOptions options = new()
        {
            Model = deploymentName,
            InputItems =
            {
                ResponseItem.CreateUserMessageItem(request.Message)
            }
        };

        ResponseResult response = client.CreateResponse(options);
        return Results.Ok(new ChatResponse(response.GetOutputText()));
    }
    catch (Exception exception)
    {
        logger.LogError(exception, "Azure OpenAI kunde inte behandla chatmeddelandet.");
        return Results.Problem(
            title: "Kunde inte få svar från AI:n",
            detail: "Kontrollera Azure-konfigurationen och försök igen.",
            statusCode: StatusCodes.Status502BadGateway);
    }
});

app.Run();

/// <summary>Innehåller användarens meddelande till AI:n.</summary>
public sealed record ChatRequest
{
    /// <summary>Användarens fråga eller instruktion.</summary>
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(4000, MinimumLength = 1)]
    public required string Message { get; init; }
}

/// <summary>Innehåller AI:ns svar.</summary>
/// En färdig liten behållare för ett AI-svar, med ett fält som heter Message.
public sealed record ChatResponse(string Message);
