using AiRagDemo.Api.Contracts;
using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Application.Abstractions.Persistence;
using AiRagDemo.Application.Processing;
using AiRagDemo.Application.Services;
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

app.MapPost("/ingest", async (
    IngestRequest request,
    TextChunker chunker,
    IEmbeddingService embeddingService,
    IChunkRepository chunkRepository,
    CancellationToken cancellationToken
    ) =>
{
    if(string.IsNullOrWhiteSpace(request.DocumentName))
        return Results.BadRequest("Document name is required.");
    
    if(string.IsNullOrWhiteSpace(request.Text))
        return Results.BadRequest("Text is required.");
    
    var chunks = chunker.Chunk(request.DocumentName, request.Text);

    foreach (var chunk in chunks)
    {
        chunk.Embedding = await embeddingService.CreateEmbeddingAsync(chunk.Content, cancellationToken);
    }
    
    await chunkRepository.AddRangeAsync(chunks, cancellationToken);
    
    return Results.Ok( new
    {
        request.DocumentName,
        ChunkCount = chunks.Count
    });
});

app.MapPost("/ask-rag", async (
    AskRagRequest request,
    RagService ragService,
    CancellationToken cancellationToken) =>
{
    if(string.IsNullOrWhiteSpace(request.Question))
        return Results.BadRequest("Question is required.");

    var result = await ragService.AskAsync(
        request.Question,
        request.TopK <= 0 ? 3 : request.TopK,
        cancellationToken);
    
    return Results.Ok(result);
});

app.Run();
