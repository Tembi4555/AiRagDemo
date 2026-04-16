namespace AiRagDemo.Infrastructure.Options;

public class OllamaPhi3Options
{
    public string BaseUrl { get; init; } = "http://localhost:11434/";
    public string EmbeddingModel { get; init; } = "nomic-embed-text";
    public string ChatModel { get; init; } = "phi3";
}