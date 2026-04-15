using AiRagDemo.Domain.Models;

namespace AiRagDemo.Application.Abstractions.Persistence;

public interface IChunkRepository
{
    Task AddRangeAsync(IEnumerable<DocumentChunk>  chunks, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DocumentChunk>> GetAllAsync(CancellationToken cancellationToken = default);
}