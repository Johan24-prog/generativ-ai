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

ResponsesClient client = new(
    model: deploymentName,
    credential: new ApiKeyCredential(apiKey),
    options: new OpenAIClientOptions
    {
        Endpoint = new Uri(endpoint)
    });

CreateResponseOptions options = new()
{
    Model = deploymentName,
    InputItems =
    {
        ResponseItem.CreateUserMessageItem(
            "Hej! Kan du berätta vad du kan hjälpa mig med?"
        )
    }
};

ResponseResult response = client.CreateResponse(options);

Console.WriteLine(response.GetOutputText());

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();



app.Run();
