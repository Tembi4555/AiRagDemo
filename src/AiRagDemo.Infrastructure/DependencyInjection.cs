using AiRagDemo.Application.Abstractions.AI;
using AiRagDemo.Infrastructure.AI.OpenAI;
using AiRagDemo.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AiRagDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiApiOptions>(configuration.GetSection("OpenAI"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AiApiOptions>>().Value);

        services.AddHttpClient<OpenAiEmbeddingService>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AiApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddScoped<IEmbeddingService, OpenAiEmbeddingService>();

        return services;
    }
}
