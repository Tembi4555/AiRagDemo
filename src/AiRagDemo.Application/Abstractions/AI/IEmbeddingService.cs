namespace AiRagDemo.Application.Abstractions.AI;

public interface IEmbeddingService
{
    Task<float[]> CreateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}