using ChatApp.ChatBox;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.SemanticKernel.Memory;

namespace ChatApp;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddChatBot(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new OpenAIOptions();
        configuration.GetSection(OpenAIOptions.SettingName)
                     .Bind(options);

        services.AddTransient<IChatBot, ChatBot>();

        services.AddTransient<IKernel>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;

            // https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/6-memory-and-embeddings.ipynb
            var config = new KernelConfig();
            config.AddOpenAIEmbeddingGeneration("ada", options.EmbeddingModel, options.ApiKey);
            config.AddOpenAITextCompletion(nameof(ChatBot), options.Model, options.ApiKey, options.Organization);            

            var logger = sp.GetService<ILogger>();
            IKernel kernel = Kernel.Builder
                .WithLogger(logger!)
                .WithConfiguration(config)
                .WithMemoryStorage(new VolatileMemoryStore())
                .Build();

            return kernel;
        });

        return services;
    }
}