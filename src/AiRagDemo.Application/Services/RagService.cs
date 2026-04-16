using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Application.Abstractions.Search;

namespace AiRagDemo.Application.Services;

/// <summary>
/// RAG orchestration service
/// </summary>
public sealed class RagService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorSearchService _vectorSearchService;
    private readonly IChatService _chatService;

    public RagService(
        IEmbeddingService embeddingService,
        IVectorSearchService vectorSearchService,
        IChatService chatService)
    {
        _embeddingService = embeddingService;
        _vectorSearchService = vectorSearchService;
        _chatService = chatService;
    }

    public async Task<RagAnswer> AskAsync(
        string question,
        int topK,
        CancellationToken cancellationToken = default)
    {
        var queryEmbedding = await _embeddingService.CreateEmbeddingAsync(question, cancellationToken);
        
        var searchResult = await _vectorSearchService.SearchAsync(
            queryEmbedding, 
            topK, 
            cancellationToken);

        var contexts = searchResult.Select(r => r.Chunk.Content).ToList();
        
        var prompt = BuildPrompt(question, contexts);
        
        var answer = await _chatService.AskAsync(prompt, cancellationToken);

        return new RagAnswer
        {
            Answer = answer,
            Sources = searchResult.Select(r => new RagSource
            {
                DocumentName = r.Chunk.DocumentName,
                ChunkIndex = r.Chunk.ChunkIndex,
                Score = r.Score
            }).ToList()
        };
    }

    private static string BuildPrompt(string question, List<string> contexts)
    {
        var joined = string.Join(
            "\n\n---\n\n",
            contexts.Select((c, i) => $"Источник {i + 1}:\n{c}"));

        return $"""
                Ты помощник, который отвечает только по предоставленному контексту.
                Если в контексте недостаточно данных, честно скажи об этом.

                Контекст:
                {joined}

                Вопрос:
                {question}
                """;
    }
}

public sealed class RagAnswer
{
    public string Answer { get; init; } = string.Empty;
    public IReadOnlyCollection<RagSource> Sources { get; init; } = [];
}

public sealed class RagSource
{
    public string DocumentName { get; init; } = string.Empty;
    public int ChunkIndex { get; init; }
    public double Score { get; init; }
}