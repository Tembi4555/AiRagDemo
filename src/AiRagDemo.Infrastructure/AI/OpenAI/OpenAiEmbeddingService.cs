using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Infrastructure.Options;

namespace AiRagDemo.Infrastructure.AI.OpenAI;

public sealed class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly AiApiOptions _options;
    private readonly HttpClient _httpClient;

    public OpenAiEmbeddingService(HttpClient httpClient, AiApiOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }
    
    public async Task<float[]> CreateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var request = new EmbeddingRequest
        {
            Model = _options.EmbeddingModel,
            Input = text
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/embeddings");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        httpRequest.Content = JsonContent.Create(request);
        
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        
        var payload = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Empty embedding response");
        
        var embedding = payload.Data.FirstOrDefault()?.Embedding
            ?? throw new InvalidOperationException("Embedding not found");
        
        return embedding;
    }
    
    private sealed class EmbeddingRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; init; } = string.Empty;
        [JsonPropertyName("input")]
        public string Input { get; init; } = string.Empty;
    }
    
    private sealed class EmbeddingResponse
    {
        [JsonPropertyName("data")] 
        public List<EmbeddingItem> Data { get; init; } = [];
    }
    
    private sealed class EmbeddingItem
    {
        [JsonPropertyName("embedding")] 
        public float[] Embedding { get; init; } = [];
    }
}


