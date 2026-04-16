namespace AiRagDemo.Infrastructure.Options;

public sealed class OpenAiOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://api.openai.com/";
    public string EmbeddingModel { get; init; } = "text-embedding-3-small";
    public string ChatModel { get; init; } = "gpt-4.1-mini";
    
}