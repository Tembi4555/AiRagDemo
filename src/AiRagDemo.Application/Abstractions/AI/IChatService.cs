namespace AiRagDemo.Application.Abstractions.AI;

public interface IChatService
{
    Task<string> AskAsync(string prompt, CancellationToken cancellationToken = default);
}