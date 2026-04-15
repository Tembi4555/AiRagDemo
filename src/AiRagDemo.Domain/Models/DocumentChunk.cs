namespace AiRagDemo.Domain.Models;

public sealed class DocumentChunk
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string DocumentName { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public int ChunkIndex { get; init; }
    public float[] Embedding { get; set; } = [];
}