﻿using ChatApp.Options;
using ChatApp.Services.ChatBot;
using ChatApp.Services.ChatBot.Implementation;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.SemanticKernel.Memory;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddChatBot(this IServiceCollection services, IConfiguration configuration)
    {
        // IOptions
        services.Configure<NLPServiceOptions>(configuration.GetSection(NLPServiceOptions.SettingName));
        services.Configure<ChatBotOptions>(configuration.GetSection(ChatBotOptions.SettingName));
        
        // Services
        services.AddTransient<IChatBot, ChatBot>();

        services.AddTransient<IKernel>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<NLPServiceOptions>>().Value;

            // https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/6-memory-and-embeddings.ipynb
            var config = new KernelConfig();
            config.AddOpenAIEmbeddingGeneration("ada", options.EmbeddingModel, options.ApiKey);
            config.AddOpenAITextCompletion(nameof(ChatBot), options.Model, options.ApiKey, options.Organization);

            var logger = sp.GetService<ILogger<IKernel>>();
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