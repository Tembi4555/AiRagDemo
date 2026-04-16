using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/ask", async (
    string question,
    IChatService chatService,
    CancellationToken cancellationToken) =>
{
    var answer = await chatService.AskAsync(question, cancellationToken);
    return Results.Ok(new {Answer = answer});
});

app.MapPost("/embed", async (
    string text,
    IEmbeddingService embeddingService, 
    CancellationToken cancellationToken) =>
{
    var vector = await embeddingService.CreateEmbeddingAsync(text, cancellationToken);
    return Results.Ok(new
    {
        vector.Length,
        Preview = vector.Take(8)
    });
});

app.Run();
