namespace AiRagDemo.Domain.Models;

public sealed class SearchResult
{
    public DocumentChunk Chunk { get; init; } = default!;
    public double Score { get; init; }
}