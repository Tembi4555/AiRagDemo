using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Application.Abstractions.Persistence;
using AiRagDemo.Application.Abstractions.Search;
using AiRagDemo.Application.Processing;
using AiRagDemo.Application.Services;
using AiRagDemo.Infrastructure.AI.Chats;
using AiRagDemo.Infrastructure.AI.Embeddings;
using AiRagDemo.Infrastructure.Options;
using AiRagDemo.Infrastructure.Persistence;
using AiRagDemo.Infrastructure.VectorSearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AiRagDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var ollamaPhi3LocalSection = configuration.GetSection("OllamaPhi3Local");
        if (!ollamaPhi3LocalSection.Exists())
        {
            throw new InvalidOperationException("OllamaPhi3Local section not found");
        }
        
        services
            .AddOptions<OllamaPhi3Options>()
            .Bind(ollamaPhi3LocalSection)
            .Validate(o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                "Ollama phi3 local url is not valid")
            .ValidateOnStart();
        
        services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>((sp, client) =>
        {
            var optsOllama = sp.GetRequiredService<IOptions<OllamaPhi3Options>>().Value;
            client.BaseAddress = new Uri(optsOllama.BaseUrl);
        });
        
        services.AddHttpClient<IChatService, OllamaChatService>((sp, client) =>
        {
            var optsOllama = sp.GetRequiredService<IOptions<OllamaPhi3Options>>().Value;
            client.BaseAddress = new Uri(optsOllama.BaseUrl);
        });

        services.AddSingleton<IChunkRepository, InMemoryChunkRepository>();
        services.AddScoped<IVectorSearchService, InMemoryVectorSearchService>();
        services.AddSingleton<TextChunker>();
        services.AddScoped<RagService>();
        
        return services;
    }
}
