using AiRagDemo.Domain.Models;

namespace AiRagDemo.Application.Abstractions.Search;

public interface IVectorSearchService
{
    Task<IReadOnlyCollection<SearchResult>> SearchAsync(
        float[] queryEmbedding, 
        int topK,
        CancellationToken cancellationToken = default);
}