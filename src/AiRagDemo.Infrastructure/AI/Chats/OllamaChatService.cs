using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace AiRagDemo.Infrastructure.AI.Chats;

public sealed class OllamaChatService : IChatService
{
    private readonly OllamaPhi3Options _options;
    private readonly HttpClient _httpClient;

    public OllamaChatService(HttpClient httpClient, IOptions<OllamaPhi3Options> options)
    {
        _httpClient = httpClient;
        _options  = options.Value;
    }
    public async Task<string> AskAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var request = new OllamaGenerateRequest
        {
            Model = _options.ChatModel,
            Prompt = prompt,
            Stream = false
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "api/generate", 
            request, 
            cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var payload = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty Ollama response");
        
        if(string.IsNullOrWhiteSpace(payload.Response))
            throw new InvalidOperationException("Ollama returned empty response");
        
        return payload.Response;
    }

    private sealed class OllamaGenerateRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; init; } = string.Empty;
        [JsonPropertyName("prompt")]
        public string Prompt { get; init; } = string.Empty;
        [JsonPropertyName("stream")]
        public bool Stream { get; init; }
    }
    
    private sealed class OllamaGenerateResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; init; } = string.Empty;
        
    }
}