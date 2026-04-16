namespace AiRagDemo.Api.Contracts;

public sealed class AskRagRequest
{
    public string Question { get; init; } = string.Empty;
    public int TopK { get; init; } = 3;
}