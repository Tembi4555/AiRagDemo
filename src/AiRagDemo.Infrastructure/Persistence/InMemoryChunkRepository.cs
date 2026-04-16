using AiRagDemo.Application.Abstractions.Persistence;
using AiRagDemo.Domain.Models;

namespace AiRagDemo.Infrastructure.Persistence;

public class InMemoryChunkRepository : IChunkRepository
{
    private readonly List<DocumentChunk> _chunks = [];
    
    public Task AddRangeAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default)
    {
        _chunks.AddRange(chunks);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<DocumentChunk>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<DocumentChunk>>(_chunks);
    }
}