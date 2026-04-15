namespace AiRagDemo.Api.Contracts;

/// <summary>
/// Endpoint для индексации текста
/// </summary>
public sealed class IngestRequest
{
    public string DocumentName { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}