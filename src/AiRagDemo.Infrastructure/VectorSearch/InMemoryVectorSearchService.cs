using AiRagDemo.Application.Abstractions.Persistence;
using AiRagDemo.Application.Abstractions.Search;
using AiRagDemo.Domain.Models;

namespace AiRagDemo.Infrastructure.VectorSearch;

/// <summary>
/// Сервис для поиска чанков в локальной памяти
/// </summary>
public sealed class InMemoryVectorSearchService : IVectorSearchService
{
    private readonly IChunkRepository _chunkRepository;

    public InMemoryVectorSearchService(IChunkRepository chunkRepository)
    {
        _chunkRepository =  chunkRepository;
    }
    public async Task<IReadOnlyCollection<SearchResult>> SearchAsync(
        float[] queryEmbedding, 
        int topK, 
        CancellationToken cancellationToken = default)
    {
        var chunks = await _chunkRepository.GetAllAsync(cancellationToken);
        
        var results = chunks
            .Where(c => c.Embedding.Length > 0)
            .Select(c => new SearchResult
            {
                Chunk = c,
                Score = VectorMath.CosinesSimilarity(queryEmbedding, c.Embedding),
            })
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .ToList();
        
        return results;
    }
}