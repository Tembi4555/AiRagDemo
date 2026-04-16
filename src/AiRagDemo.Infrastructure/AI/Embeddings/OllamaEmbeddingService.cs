using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace AiRagDemo.Infrastructure.AI.Embeddings;

public sealed class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaPhi3Options _options;

    public OllamaEmbeddingService(HttpClient httpClient, IOptions<OllamaPhi3Options> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    
    public async Task<float[]> CreateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var request = new OllamaEmbeddingRequest
        {
            Model = _options.EmbeddingModel,
            Input = text,
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "api/embed",
            request,
            cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var payload = await response.Content.ReadFromJsonAsync<OllamaEmbeddingsResponse>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty Ollama embedding response.");
        
        var embedding = payload.Embeddings.FirstOrDefault()
            ?? throw new InvalidOperationException("Embedding not found.");
        
        return embedding;
    }
    
    private sealed class OllamaEmbeddingRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; init; } = string.Empty;
        [JsonPropertyName("input")]
        public string Input { get; init; } = string.Empty;
    }
    
    private sealed class OllamaEmbeddingsResponse
    {
        [JsonPropertyName("embeddings")] 
        public List<float[]> Embeddings { get; init; } = [];
    }
}